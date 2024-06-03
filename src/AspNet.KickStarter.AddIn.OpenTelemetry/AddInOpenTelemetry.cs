using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Metrics;
using System.Security.Principal;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the OpenTelemetry add-in.
/// </summary>
internal class AddInOpenTelemetry : IAddIn
{
    private string? _applicationName;

    /// <summary>
    /// Gets a unique ID to be used to identify this running instance of the service.
    /// </summary>
    public static string ServiceInstanceId { get; } = $"{Environment.MachineName}-{Convert.ToBase64String(Guid.NewGuid().ToByteArray()).TrimEnd('=').Replace('/', '-').Replace('+', '-')}";

    /// <summary>
    /// Gets or sets the port the metrics HTTP listener should use. The default is 8081.
    /// </summary>
    internal ushort? PrometheusPort { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional trace configuration such as AddSource.
    /// </summary>
    internal Action<TracerProviderBuilder>? ConfigureTraceBuilder { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional metrics configuration such as AddMeter or AddView.
    /// </summary>
    internal Action<MeterProviderBuilder>? ConfigureMetricsBuilder { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional logger configuration such as AddFilter.
    /// </summary>
    internal Action<OpenTelemetryLoggerOptions>? ConfigureLoggerOptions { get; set; }

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns)
    {
        _applicationName = builder.Environment.ApplicationName;
        builder.Services.AddOpenTelemetry()
                        .ConfigureResource(ConfigureResource)
                        .WithTracing(ConfigureTracing)
                        .WithMetrics(ConfigureMetrics);

        // If Serilog addin is applied then it will use its own Sink for logging to OpenTelemetry
        if (addIns.All(_ => _.GetType().Name != "AddInSerilog"))
            builder.Logging.AddOpenTelemetry(ConfigureLogging);

        builder.Services.AddSingleton<ITraceActivity>(_ => new TraceActivity(_applicationName));
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns) { }

    /// <summary>
    /// Configure the service name and instance id.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    internal void ConfigureResource(ResourceBuilder builder)
        => builder.AddService(_applicationName!, serviceInstanceId: ServiceInstanceId);

    /// <summary>
    /// Configure the desired tracing instruments and exporters.
    /// </summary>
    /// <param name="otel">The builder to configure.</param>
    internal void ConfigureTracing(TracerProviderBuilder otel)
    {
        otel.AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddSource(_applicationName!);

        var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_TRACES_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

        if (!string.IsNullOrWhiteSpace(endpoint))
            otel.AddOtlpExporter();
        else
            otel.AddConsoleExporter();

        ConfigureTraceBuilder?.Invoke(otel);
    }

    /// <summary>
    /// Configure the desired metrics instrumentation and exporters.
    /// </summary>
    /// <param name="otel">The builder to configure.</param>
    internal void ConfigureMetrics(MeterProviderBuilder otel)
    {
        otel.AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddMeter("System.Net.Http")
            .AddMeter(_applicationName!);

        SetDefaultHistogramBuckets(otel);

        var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_METRICS_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

        if (!string.IsNullOrWhiteSpace(endpoint))
            otel.AddOtlpExporter();
        else
            otel.AddConsoleExporter();

        if (PrometheusPort is not null)
        {
            otel.AddPrometheusHttpListener(config =>
            {
                config.ScrapeEndpointPath = "/";
                config.UriPrefixes = [$"http://*:{PrometheusPort}/"];
                UseLocalhostForWindowsNonAdmin(config);
            });
        }

        ConfigureMetricsBuilder?.Invoke(otel);
    }

    [ExcludeFromCodeCoverage]
    private static void SetDefaultHistogramBuckets(MeterProviderBuilder otel)
    {
        // By default, the boundaries used for a Histogram are { 0, 5, 10, 25, 50, 75, 100, 250, 500, 750, 1000, 2500, 5000, 7500, 10000 }
        // Those default values are very high for millisecond timings which are assumed to be the normal metrics recorded by the applications that use this class.
        // 15 buckets are also a lot to fit into a Grafana Bar Gauge panel.
        // The buckets used by default here range from 1μs to 1 second with increasing pseudo-exponential widths.
        // If custom values are required then they can be configured for named histogram by passing a configureMetricsBuilder action to WithOpenTelemetry().
        // See https://github.com/open-telemetry/opentelemetry-dotnet/blob/main/docs/metrics/customizing-the-sdk/README.md#explicit-bucket-histogram-aggregation
        var defaultConfig = new ExplicitBucketHistogramConfiguration { Boundaries = [0.001, 0.005, 0.1, 0.5, 1, 10, 125, 250, 500, 1000] };
        otel.AddView(instrument => (instrument is Histogram<double>) ? defaultConfig : null);
    }

    [ExcludeFromCodeCoverage(Justification = "Environment & user specific code")]
    private void UseLocalhostForWindowsNonAdmin(PrometheusHttpListenerOptions config)
    {
        // On Windows when not administrator access is denied to listen on http://*:8081
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        {
#pragma warning disable CA1416 // Validate platform compatibility
#pragma warning disable S1066 // Mergeable "if" statements should be combined
            if (!new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
                config.UriPrefixes = [$"http://localhost:{PrometheusPort}/"];
#pragma warning restore S1066 // Mergeable "if" statements should be combined
#pragma warning restore CA1416 // Validate platform compatibility
        }
    }

    /// <summary>
    /// Configure the desired logging exporters.
    /// </summary>
    /// <param name="otel">The builder to configure.</param>
    internal void ConfigureLogging(OpenTelemetryLoggerOptions otel)
    {
        // Export the body of the message
        otel.IncludeFormattedMessage = true;

        otel.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(_applicationName!));

        var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT")
            ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

        if (!string.IsNullOrWhiteSpace(endpoint))
            otel.AddOtlpExporter();
        else
            otel.AddConsoleExporter();

        ConfigureLoggerOptions?.Invoke(otel);
    }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class MetricsAddInExtensions
{
    /// <summary>
    /// Add OpenTelemetry with an optional prometheus metrics scraper path.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Serilog to.</param>
    /// <param name="prometheusPort">The optional port the metrics HTTP listener should use. For example, 8081.</param>
    /// <param name="configureTraceBuilder">An optional action for additional trace configuration such as AddSource.</param>
    /// <param name="configureMetricsBuilder">An optional action for additional metrics configuration such as AddMeter or AddView.</param>
    /// <param name="configureLoggerOptions">An optional action for additional logger configuration such as AddFilter.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithOpenTelemetry(
        this ApiBuilder apiBuilder,
        ushort? prometheusPort = null,
        Action<TracerProviderBuilder>? configureTraceBuilder = null,
        Action<MeterProviderBuilder>? configureMetricsBuilder = null,
        Action<OpenTelemetryLoggerOptions>? configureLoggerOptions = null)
    {
        apiBuilder.RegisterAddIn(new AddInOpenTelemetry
        {
            PrometheusPort = prometheusPort,
            ConfigureTraceBuilder = configureTraceBuilder,
            ConfigureMetricsBuilder = configureMetricsBuilder,
            ConfigureLoggerOptions = configureLoggerOptions
        });

        return apiBuilder;
    }
}
