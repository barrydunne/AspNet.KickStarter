using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;

namespace AspNet.KickStarter.AddIn.OpenTelemetry.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInOpenTelemetryTests
{
    private readonly Fixture _fixture;
    private readonly ServiceCollection _serviceCollection;
    private readonly WebApplicationBuilder _builder;
    private readonly AddInOpenTelemetry _sut;

    public AddInOpenTelemetryTests()
    {
        _fixture = new();
        _serviceCollection = new ServiceCollection();
        _builder = WebApplication.CreateBuilder(Array.Empty<string>());
        _builder.SetServices(_serviceCollection);
        _sut = _fixture.Create<AddInOpenTelemetry>();
    }

    [Test]
    public void AddInOpenTelemetry_adds_services()
    {
        _sut.Configure(_builder, []);

        Assert.That(_serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType.Namespace == "OpenTelemetry.Metrics"), "Missing Metrics");
        Assert.That(_serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType.Namespace == "OpenTelemetry.Trace"), "Missing Trace");
        Assert.That(_builder.Logging.Services, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType.FullName!.Contains("OpenTelemetryLoggerOptions")), "Missing Logs");
    }

    [Test]
    public void AddInOpenTelemetry_with_serilog_adds_services_without_logger()
    {
        var addInSerilog = new AddInSerilog();
        _sut.Configure(_builder, [addInSerilog]);

        Assert.That(_serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType.Namespace == "OpenTelemetry.Metrics"), "Missing Metrics");
        Assert.That(_serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType.Namespace == "OpenTelemetry.Trace"), "Missing Trace");
        Assert.That(_builder.Logging.Services, Has.None.Matches<ServiceDescriptor>(_ => _.ServiceType.FullName!.Contains("OpenTelemetryLoggerOptions")), "Found Logs");
    }

    [Test]
    public void WithOpenTelemetry_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithOpenTelemetry();

        var addin = apiBuilder.GetAddIn<AddInOpenTelemetry>();
        Assert.That(addin?.PrometheusPort, Is.Null, "Incorrect PrometheusPort");
        Assert.That(addin?.ConfigureTraceBuilder, Is.Null, "Incorrect ConfigureTraceBuilder");
        Assert.That(addin?.ConfigureMetricsBuilder, Is.Null, "Incorrect ConfigureMetricsBuilder");
        Assert.That(addin?.ConfigureLoggerOptions, Is.Null, "Incorrect ConfigureLoggerOptions");
    }

    [Test]
    public void WithOpenTelemetry_registers_add_in_with_custom_properties()
    {
        var port = _fixture.Create<ushort>();
        var additionalNames = _fixture.CreateMany<string>().ToArray();
        var apiBuilder = new ApiBuilder();
        static void ConfigureTraceBuilder(TracerProviderBuilder builder) => throw new NotImplementedException();
        static void ConfigureMetricsBuilder(MeterProviderBuilder builder) => throw new NotImplementedException();
        static void ConfigureLoggerOptions(OpenTelemetryLoggerOptions options) => throw new NotImplementedException();

        apiBuilder.WithOpenTelemetry(port, ConfigureTraceBuilder, ConfigureMetricsBuilder, ConfigureLoggerOptions);

        var addin = apiBuilder.GetAddIn<AddInOpenTelemetry>();
        Assert.That(addin?.PrometheusPort, Is.EqualTo(port), "Incorrect PrometheusPort");
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(addin?.ConfigureTraceBuilder, Is.EqualTo(ConfigureTraceBuilder), "Incorrect ConfigureTraceBuilder");
        Assert.That(addin?.ConfigureMetricsBuilder, Is.EqualTo(ConfigureMetricsBuilder), "Incorrect ConfigureMetricsBuilder");
        Assert.That(addin?.ConfigureLoggerOptions, Is.EqualTo(ConfigureLoggerOptions), "Incorrect ConfigureLoggerOptions");
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }

    [Test]
    public void Configure_calls_ConfigureTraceBuilder()
    {
        var invoked = false;
        void ConfigureTraceBuilder(TracerProviderBuilder builder) => invoked = true;
        _sut.ConfigureTraceBuilder = ConfigureTraceBuilder;

        _sut.Configure(_builder, []);

        Assert.That(invoked, Is.True);
    }

    [Test]
    public void Configure_calls_ConfigureMetricsBuilder()
    {
        var invoked = false;
        void ConfigureMetricsBuilder(MeterProviderBuilder builder) => invoked = true;
        _sut.ConfigureMetricsBuilder = ConfigureMetricsBuilder;

        _sut.Configure(_builder, []);

        Assert.That(invoked, Is.True);
    }

    [Test]
    public void Configure_calls_ConfigureLoggerOptions()
    {
        var invoked = false;
        void ConfigureLoggerOptions(OpenTelemetryLoggerOptions options) => invoked = true;
        _sut.ConfigureLoggerOptions = ConfigureLoggerOptions;

        _sut.Configure(_builder, []);
        _sut.ConfigureLogging(new());

        Assert.That(invoked, Is.True);
    }

    [Test]
    public void ServiceInstanceId_starts_with_machine_name()
        => Assert.That(AddInOpenTelemetry.ServiceInstanceId, Does.StartWith(Environment.MachineName));

    [Test]
    public void ConfigureResource_sets_service_name()
    {
        _sut.Configure(_builder, []);
        var resourceBuilder = ResourceBuilder.CreateDefault();

        _sut.ConfigureResource(resourceBuilder);

        var resources = resourceBuilder.GetType().GetField("ResourceDetectors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(resourceBuilder) as List<IResourceDetector>;
        var wrapper = resources?.LastOrDefault(_ => _.GetType().Name == "WrapperResourceDetector");
        var resource = wrapper?.GetType().GetField("resource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(wrapper) as Resource;
        var value = resource?.Attributes.FirstOrDefault(_ => _.Key == "service.name").Value;
        Assert.That(value, Is.EqualTo(_builder.Environment.ApplicationName));
    }

    [Test]
    public void ConfigureResource_sets_service_instance_id()
    {
        _sut.Configure(_builder, []);
        var resourceBuilder = ResourceBuilder.CreateDefault();

        _sut.ConfigureResource(resourceBuilder);

        var resources = resourceBuilder.GetType().GetField("ResourceDetectors", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(resourceBuilder) as List<IResourceDetector>;
        var wrapper = resources?.LastOrDefault(_ => _.GetType().Name == "WrapperResourceDetector");
        var resource = wrapper?.GetType().GetField("resource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(wrapper) as Resource;
        var value = resource?.Attributes.FirstOrDefault(_ => _.Key == "service.instance.id").Value;
        Assert.That(value, Is.EqualTo(AddInOpenTelemetry.ServiceInstanceId));
    }

    [Test]
    public void ConfigureTracing_with_otlp_traces_endpoint_adds_otlp_exporter()
    {
        var trace = new TracerProviderBuilderBase();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", _fixture.Create<string>());

        _sut.ConfigureTracing(trace);

        var exporter = GetExporter(trace);
        Assert.That(exporter, Is.TypeOf<OtlpTraceExporter>());
    }

    [Test]
    public void ConfigureTracing_with_otlp_endpoint_adds_otlp_exporter()
    {
        var trace = new TracerProviderBuilderBase();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);

        _sut.ConfigureTracing(trace);

        var exporter = GetExporter(trace);
        Assert.That(exporter, Is.TypeOf<OtlpTraceExporter>());
    }

    [Test]
    public void ConfigureTracing_without_otlp_endpoint_adds_console_exporter()
    {
        var trace = new TracerProviderBuilderBase();
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);

        _sut.ConfigureTracing(trace);

        var exporter = GetExporter(trace);
        Assert.That(exporter, Is.TypeOf<ConsoleActivityExporter>());
    }

    private static object? GetExporter(TracerProviderBuilder trace)
    {
        var innerBuilder = trace.GetType().GetField("innerBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(trace);
        var services = innerBuilder!.GetType().GetProperty("Services")!.GetValue(innerBuilder) as IServiceCollection;
        var sp = services!.BuildServiceProvider();
        var sdkType = services!.FirstOrDefault(_ => _.ServiceType.FullName == "OpenTelemetry.Trace.TracerProviderBuilderSdk")!.ServiceType;
        var sdk = sp.GetService(sdkType);
        var configType = services!.FirstOrDefault(_ => _.ServiceType.FullName == "OpenTelemetry.Trace.IConfigureTracerProviderBuilder")!.ServiceType;
        var configs = sp!.GetServices(configType!);
        var config = configs.LastOrDefault();
        config!.GetType().GetMethod("ConfigureBuilder", new[] { typeof(IServiceProvider), typeof(TracerProviderBuilder) })!.Invoke(config, [sp, sdk]);
        var processors = sdkType.GetProperty("Processors")!.GetValue(sdk) as List<BaseProcessor<Activity>>;
        var processor = processors!.First();
        var exporter = processor.GetType().GetField("exporter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(processor);
        return exporter;
    }

    [Test]
    public void ConfigureMetrics_with_otlp_metrics_endpoint_adds_otlp_exporter()
    {
        var metrics = new MeterProviderBuilderBase();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", _fixture.Create<string>());

        _sut.ConfigureMetrics(metrics);

        var exporter = GetExporter(metrics);
        Assert.That(exporter, Is.TypeOf<OtlpMetricExporter>());
    }

    [Test]
    public void ConfigureMetrics_with_otlp_endpoint_adds_otlp_exporter()
    {
        var metrics = new MeterProviderBuilderBase();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);

        _sut.ConfigureMetrics(metrics);

        var exporter = GetExporter(metrics);
        Assert.That(exporter, Is.TypeOf<OtlpMetricExporter>());
    }

    [Test]
    public void ConfigureMetrics_without_otlp_endpoint_adds_console_exporter()
    {
        var metrics = new MeterProviderBuilderBase();
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);

        _sut.ConfigureMetrics(metrics);

        var exporter = GetExporter(metrics);
        Assert.That(exporter, Is.TypeOf<ConsoleMetricExporter>());
    }

    private static object? GetExporter(MeterProviderBuilderBase metrics)
    {
        var innerBuilder = metrics.GetType().GetField("innerBuilder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(metrics);
        var services = innerBuilder!.GetType().GetProperty("Services")!.GetValue(innerBuilder) as IServiceCollection;
        var sp = services!.BuildServiceProvider();
        var sdkType = services!.FirstOrDefault(_ => _.ServiceType.FullName == "OpenTelemetry.Metrics.MeterProviderBuilderSdk")!.ServiceType;
        var sdk = sp.GetService(sdkType);
        var configType = services!.FirstOrDefault(_ => _.ServiceType.FullName == "OpenTelemetry.Metrics.IConfigureMeterProviderBuilder")!.ServiceType;
        var configs = sp!.GetServices(configType!);
        var config = configs.LastOrDefault();
        config!.GetType().GetMethod("ConfigureBuilder", new[] { typeof(IServiceProvider), typeof(MeterProviderBuilder) })!.Invoke(config, [sp, sdk]);
        var readers = sdkType.GetProperty("Readers")!.GetValue(sdk) as List<MetricReader>;
        var reader = readers!.First();
        var exporter = reader.GetType().GetField("exporter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(reader);
        return exporter;
    }

    [Test]
    public void ConfigureMetrics_with_prometheus_port_adds_listener()
    {
        _sut.PrometheusPort = 0;
        var metrics = new MeterProviderBuilderBase();

        _sut.ConfigureMetrics(metrics);

        Assert.That(() => GetExporter(metrics), Throws.Exception.With.InnerException.With.Message.Contains("PrometheusExporter HttpListener could not be started."));
    }

    [Test]
    public void ConfigureLogging_with_otlp_metrics_endpoint_adds_otlp_exporter()
    {
        var options = new OpenTelemetryLoggerOptions();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", endpoint);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", _fixture.Create<string>());
        _sut.Configure(_builder, []);

        _sut.ConfigureLogging(options);

        var exporter = GetExporter(options);
        Assert.That(exporter, Is.TypeOf<OtlpLogExporter>());
    }

    [Test]
    public void ConfigureLogging_with_otlp_endpoint_adds_otlp_exporter()
    {
        var options = new OpenTelemetryLoggerOptions();
        var endpoint = $"http://localhost:{_fixture.Create<ushort>()}/";
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", endpoint);
        _sut.Configure(_builder, []);

        _sut.ConfigureLogging(options);

        var exporter = GetExporter(options);
        Assert.That(exporter, Is.TypeOf<OtlpLogExporter>());
    }

    [Test]
    public void ConfigureLogging_without_otlp_endpoint_adds_console_exporter()
    {
        var options = new OpenTelemetryLoggerOptions();
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT", null);
        Environment.SetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT", null);
        _sut.Configure(_builder, []);

        _sut.ConfigureLogging(options);

        var exporter = GetExporter(options);
        Assert.That(exporter, Is.TypeOf<ConsoleLogRecordExporter>());
    }

    private static object? GetExporter(OpenTelemetryLoggerOptions options)
    {
        var processorFactories = options.GetType().GetField("ProcessorFactories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(options) as List<Func<IServiceProvider, BaseProcessor<LogRecord>>>;
        var processorFactory = processorFactories!.First();
        var target = processorFactory.Target;
        if (target!.GetType().FullName!.Contains("OtlpLogExporter"))
            return new OtlpLogExporter(new());
        var processor = target!.GetType().GetField("processor")!.GetValue(target);
        var exporter = processor!.GetType().GetField("exporter", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.GetValue(processor);
        return exporter;
    }
}
