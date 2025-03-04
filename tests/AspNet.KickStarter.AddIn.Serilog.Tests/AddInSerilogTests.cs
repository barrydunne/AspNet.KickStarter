using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog;
using Serilog.Core;
using System.Reflection;

namespace AspNet.KickStarter.AddIn.Serilog.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
[NonParallelizable]
internal class AddInSerilogTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void AddInSerilog_adds_services()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var sut = new AddInSerilog { DebugOutput = (_) => { } };

        sut.Configure(builder, []);

        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType == typeof(global::Serilog.ILogger)));
    }

    [Test]
    public void AddInSerilog_invokes_logger_configuration()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var invoked = 0;
        var sut = new AddInSerilog { LoggerConfiguration = (_) => invoked++ };

        sut.Configure(builder, []);

        using var provider = serviceCollection.BuildServiceProvider();
        var logger = provider.GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(invoked, Is.GreaterThan(0));
    }

    [Test]
    public void WithSerilog_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithSerilog();

        var addin = apiBuilder.GetAddIn<AddInSerilog>();
        Assert.That(addin?.DebugOutput, Is.Null);
    }

    [Test]
    public void WithSerilog_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();
        static void LoggerConfiguration(LoggerConfiguration _) => throw new NotImplementedException();
        static void DebugOutput(string _) => throw new NotImplementedException();

        apiBuilder.WithSerilog(LoggerConfiguration, DebugOutput);

        var addin = apiBuilder.GetAddIn<AddInSerilog>();
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInSerilog>()?.LoggerConfiguration, Is.EqualTo(LoggerConfiguration));
        Assert.That(apiBuilder.GetAddIn<AddInSerilog>()?.DebugOutput, Is.EqualTo(DebugOutput));
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }

    [Test]
    public void WithSerilog_does_not_write_to_opentelemetry_if_no_addin()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        var sut = new AddInSerilog { DebugOutput = (_) => { } };

        sut.Configure(builder, []);

        using var provider = serviceCollection.BuildServiceProvider();
        var logger = provider.GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, false), Is.False);
        (logger as IDisposable)?.Dispose();
    }

    [Test]
    public void WithSerilog_writes_to_opentelemetry_if_required()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var openTelemetry = new AddInOpenTelemetry();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        var sut = new AddInSerilog { DebugOutput = (_) => { } };

        sut.Configure(builder, [openTelemetry]);

        using var provider = serviceCollection.BuildServiceProvider();
        var logger = provider.GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, false), Is.True);
        (logger as IDisposable)?.Dispose();
    }

    [Test]
    public void WithSerilog_writes_to_opentelemetry_grpc_if_required()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var openTelemetry = new AddInOpenTelemetry();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc");
        var sut = new AddInSerilog { DebugOutput = (_) => { } };

        sut.Configure(builder, [openTelemetry]);

        using var provider = serviceCollection.BuildServiceProvider();
        var logger = provider.GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, true), Is.True);
        (logger as IDisposable)?.Dispose();
    }

    private bool WritesToOpenTelemetry(ILogEventSink? sink, string endpoint, bool grpc)
    {
        if (sink is null)
            return false;

        if (sink.GetType().Name == "OpenTelemetrySink")
        {
            try
            {
                var exporter = sink.GetType().GetField("_exporter", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(sink);
                var client = exporter?.GetType().GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(exporter);
                if (grpc)
                {
                    if (exporter?.GetType().Name == "GrpcExporter")
                    {
                        var channel = exporter?.GetType().GetField("_channel", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(exporter)
                                   ?? exporter?.GetType().GetField("_tracesChannel", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(exporter);
                        var address = channel?.GetType().GetProperty("Address", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(channel);
                        Assert.That(address?.ToString(), Is.EqualTo(endpoint));
                        return true;
                    }
                }
                else
                {
                    if (exporter?.GetType().Name == "HttpExporter")
                    {
                        if ((client as HttpClient)?.BaseAddress is null)
                        {
                            var logsEndpoint = exporter?.GetType().GetField("_logsEndpoint", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(exporter);
                            var tracesEndpoint = exporter?.GetType().GetField("_tracesEndpoint", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(exporter);
                            Assert.That(logsEndpoint, Is.EqualTo($"{endpoint}v1/logs"));
                            Assert.That(tracesEndpoint, Is.EqualTo($"{endpoint}v1/traces"));
                        }
                        else
                            Assert.That((client as HttpClient)?.BaseAddress?.ToString(), Is.EqualTo(endpoint));
                        return true;
                    }
                }
            }
            catch { }
        }

        try
        {
            var innerSink = sink!.GetType().GetField("_sink", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sink) as ILogEventSink;
            if (WritesToOpenTelemetry(innerSink, endpoint, grpc))
                return true;
        }
        catch { }
        try
        {
            var targetSink = sink!.GetType().GetField("_targetSink", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sink) as ILogEventSink;
            if (WritesToOpenTelemetry(targetSink, endpoint, grpc))
                return true;
        }
        catch { }
        try
        {
            var sinks = sink!.GetType().GetField("_sinks", BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(sink) as ILogEventSink[];
            if (sinks is not null)
            {
                foreach (var item in sinks)
                {
                    if (WritesToOpenTelemetry(item, endpoint, grpc))
                        return true;
                }
            }
        }
        catch { }

        return false;
    }
}
