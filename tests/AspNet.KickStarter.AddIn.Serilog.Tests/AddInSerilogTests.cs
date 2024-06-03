using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Serilog.Core;

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
        static void DebugOutput(string s) => throw new NotImplementedException();

        apiBuilder.WithSerilog(DebugOutput);

        var addin = apiBuilder.GetAddIn<AddInSerilog>();
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInSerilog>()?.DebugOutput, Is.EqualTo(DebugOutput));
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }

    [Test]
    public void WithSerilog_does_not_write_to_opentelemetry_if_no_addin()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var sut = new AddInSerilog { DebugOutput = (_) => { } };
        
        sut.Configure(builder, []);
        
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        var logger = serviceCollection.BuildServiceProvider().GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, false), Is.False);
    }

    [Test]
    public void WithSerilog_writes_to_opentelemetry_if_required()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var openTelemetry = new AddInOpenTelemetry();
        var sut = new AddInSerilog { DebugOutput = (_) => { } };
        
        sut.Configure(builder, [openTelemetry]);
        
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "http/protobuf");
        var logger = serviceCollection.BuildServiceProvider().GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, false), Is.True);
    }

    [Test]
    public void WithSerilog_writes_to_opentelemetry_grpc_if_required()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var openTelemetry = new AddInOpenTelemetry();
        var sut = new AddInSerilog { DebugOutput = (_) => { } };

        sut.Configure(builder, [openTelemetry]);

        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL", "grpc");
        var logger = serviceCollection.BuildServiceProvider().GetService<global::Serilog.ILogger>() as ILogEventSink;
        Assert.That(WritesToOpenTelemetry(logger, endpoint, true), Is.True);
    }

    private bool WritesToOpenTelemetry(ILogEventSink? sink, string endpoint, bool grpc)
    {
        if (sink is null)
            return false;

        if (sink.GetType().Name == "OpenTelemetrySink")
        {
            try
            {
                var exporter = sink.GetType().GetField("_exporter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(sink);
                var client = exporter?.GetType().GetField("_client", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(exporter);
                if (grpc)
                {
                    if (exporter?.GetType().Name == "GrpcExporter")
                    {
                        var channel = exporter?.GetType().GetField("_channel", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(exporter);
                        var address = channel?.GetType().GetProperty("Address", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(channel);
                        Assert.That(address?.ToString(), Is.EqualTo(endpoint));
                        return true;
                    }
                }
                else
                {
                    if (exporter?.GetType().Name == "HttpExporter")
                    {
                        Assert.That((client as HttpClient)?.BaseAddress?.ToString(), Is.EqualTo(endpoint));
                        return true;
                    }
                }
            }
            catch { }
        }

        try
        {
            var innerSink = sink!.GetType().GetField("_sink", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(sink) as ILogEventSink;
            if (WritesToOpenTelemetry(innerSink, endpoint, grpc))
                return true;
        }
        catch { }
        try
        {
            var targetSink = sink!.GetType().GetField("_targetSink", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(sink) as ILogEventSink;
            if (WritesToOpenTelemetry(targetSink, endpoint, grpc))
                return true;
        }
        catch { }
        try
        {
            var sinks = sink!.GetType().GetField("_sinks", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(sink) as ILogEventSink[];
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
