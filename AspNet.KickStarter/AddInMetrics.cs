using Microsoft.AspNetCore.Builder;
using Prometheus;
using Prometheus.HttpMetrics;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Metrics add-in.
/// </summary>
internal class AddInMetrics : IAddIn
{
    // The Prometheus default histogram buckets, which are Histogram.ExponentialBuckets(0.01, 2, 25)
    // result in values 0.01, 0.02, 0.04, 0.08, 0.16, ... 83886.08, 167772.16
    // Those default values are very high for millisecond timings which are assumed to be the normal metrics recorded by the applications that use this class.
    // 25 buckets are also a lot to fit into a Grafana Bar Gauge panel.
    // The buckets used by default here range from 1μs to 1 second with increasing pseudo-exponential widths.
    // If custom values are required then they can be configured by passing a metricsMeterAdapterOptions action to WithMetrics() that changes ResolveHistogramBuckets.
    private static readonly Action<MeterAdapterOptions> _metricsMeterAdapterDefaultOptions = (_) => _.ResolveHistogramBuckets = (_) => new[] { 0.001, 0.005, 0.1, 0.5, 1, 10, 125, 250, 500, 1000 };

    /// <summary>
    /// Gets or sets the port the metrics HTTP listener should use. The default is 8081.
    /// </summary>
    internal ushort Port { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional metrics HTTP listener configuration.
    /// </summary>
    internal Action<KestrelMetricServerOptions>? PortOptionsCallback { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional metrics export configuration.
    /// </summary>
    internal Action<HttpMiddlewareExporterOptions>? ExporterOptions { get; set; }

    /// <summary>
    /// Gets or sets an optional action for additional metrics adapter configuration such as using a custom ResolveHistogramBuckets function.
    /// </summary>
    internal Action<MeterAdapterOptions>? MeterAdapterOptions { get; set; }

    /// <summary>
    /// Configure the metrics Kestrel server options when required.
    /// </summary>
    /// <param name="options">The options to configure.</param>
    internal void OptionsCallback(KestrelMetricServerOptions options)
    {
        options.Port = Port;
        PortOptionsCallback?.Invoke(options);
    }

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder)
    {
        Metrics.ConfigureMeterAdapter(MeterAdapterOptions ?? _metricsMeterAdapterDefaultOptions);
        builder.Services.AddMetricServer(options => OptionsCallback(options));
    }

    /// <inheritdoc/>
    public void Configure(WebApplication app)
    {
        var options = new HttpMiddlewareExporterOptions();
        ExporterOptions?.Invoke(options);
        app.UseHttpMetrics(options);
    }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class MetricsAddInExtensions
{
    /// <summary>
    /// Run a Prometheus metrics exporter on a separate port.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Serilog to.</param>
    /// <param name="metricsPort">The port the metrics HTTP listener should use. The default is 8081.</param>
    /// <param name="listenerOptionsCallback">The optional action for additional metrics HTTP listener configuration.</param>
    /// <param name="metricsExporterOptions">The optional action for additional metrics export configuration.</param>
    /// <param name="metricsMeterAdapterOptions">The optional action for additional metrics adapter configuration such as using a custom ResolveHistogramBuckets function.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithMetrics(this ApiBuilder apiBuilder, ushort metricsPort = 8081, Action<KestrelMetricServerOptions>? listenerOptionsCallback = null, Action<HttpMiddlewareExporterOptions>? metricsExporterOptions = null, Action<MeterAdapterOptions>? metricsMeterAdapterOptions = null)
    {
        apiBuilder.RegisterAddIn(new AddInMetrics
        {
            Port = metricsPort,
            PortOptionsCallback = listenerOptionsCallback,
            ExporterOptions = metricsExporterOptions,
            MeterAdapterOptions = metricsMeterAdapterOptions
        });

        return apiBuilder;
    }
}
