using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Sinks.OpenTelemetry;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Serilog add-in.
/// </summary>
internal class AddInSerilog : IAddIn
{
    /// <summary>
    /// Gets or sets an optional action to configure the logger.
    /// </summary>
    internal Action<LoggerConfiguration>? LoggerConfiguration { get; set; } = null;

    /// <summary>
    /// Gets or sets an optional action to invoke with Serilog self-log messages.
    /// </summary>
    internal Action<string>? DebugOutput { get; set; } = null;

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns)
    {
        if (DebugOutput is not null)
            Serilog.Debugging.SelfLog.Enable(_ => DebugOutput.Invoke(_));

        builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration);
            var addInOpenTelemetry = addIns.FirstOrDefault(_ => _.GetType().Name == "AddInOpenTelemetry");
            if (addInOpenTelemetry is not null)
            {
                var endpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_LOGS_ENDPOINT")
                    ?? Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

                if (endpoint is not null)
                {
                    var protocol = "http/protobuf".Equals(Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_PROTOCOL"), StringComparison.OrdinalIgnoreCase)
                                 ? OtlpProtocol.HttpProtobuf
                                 : OtlpProtocol.Grpc;

                    var property = addInOpenTelemetry.GetType().GetProperty("ServiceInstanceId", BindingFlags.Static | BindingFlags.Public);
                    var serviceInstanceId = property!.GetValue(null) as string;

                    loggerConfiguration.WriteTo.OpenTelemetry(otel =>
                    {
                        otel.Endpoint = endpoint;
                        otel.Protocol = protocol;
                        otel.ResourceAttributes = new Dictionary<string, object>
                        {
                            ["service"] = builder.Environment.ApplicationName,
                            ["service.name"] = builder.Environment.ApplicationName,
                            ["service.instance.id"] = serviceInstanceId!
                        };
                    });
                }
            }
            LoggerConfiguration?.Invoke(loggerConfiguration);
        });
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns) { }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class SerilogAddInExtensions
{
    /// <summary>
    /// Use Serilog for logging in the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Serilog to.</param>
    /// <param name="configure">An optional action to apply additional logger configuration.</param>
    /// <param name="debugOutput">An optional action to invoke with Serilog self-log messages.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithSerilog(
        this ApiBuilder apiBuilder,
        Action<LoggerConfiguration>? configure = null,
        Action<string>? debugOutput = null)
    {
        apiBuilder.RegisterAddIn(new AddInSerilog
        {
            LoggerConfiguration = configure,
            DebugOutput = debugOutput
        });

        return apiBuilder;
    }
}
