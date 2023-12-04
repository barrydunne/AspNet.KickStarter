using Microsoft.AspNetCore.Builder;
using Serilog;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Serilog add-in.
/// </summary>
internal class AddInSerilog : IAddIn
{
    /// <summary>
    /// Gets or sets an optional action to invoke with Serilog self-log messages.
    /// </summary>
    internal Action<string>? DebugOutput { get; set; } = null;

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder)
    {
        if (DebugOutput is not null)
            Serilog.Debugging.SelfLog.Enable(_ => DebugOutput.Invoke(_));
        builder.Host.UseSerilog((hostBuilderContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostBuilderContext.Configuration));
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app) { }
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
    /// <param name="debugOutput">An optional action to invoke with Serilog self-log messages.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithSerilog(this ApiBuilder apiBuilder, Action<string>? debugOutput = null)
    {
        apiBuilder.RegisterAddIn(new AddInSerilog
        {
            DebugOutput = debugOutput
        });

        return apiBuilder;
    }
}
