using AspNet.KickStarter.HttpHandlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Abstractions;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the HealthHandler add-in.
/// </summary>
internal class AddInHealthHandler : IAddIn
{
    /// <summary>
    /// Gets or sets the route for the status endpoint.
    /// </summary>
    internal required string StatusEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the status endpoint name.
    /// </summary>
    internal required string StatusName { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the status endpoint.
    /// </summary>
    internal required string StatusDescription { get; set; }

    /// <summary>
    /// Gets or sets the route for the version endpoint.
    /// </summary>
    internal required string VersionEndpoint { get; set; }

    /// <summary>
    /// Gets or sets the version endpoint name.
    /// </summary>
    internal required string VersionName { get; set; }

    /// <summary>
    /// Gets or sets the detailed description of the version endpoint.
    /// </summary>
    internal required string VersionDescription { get; set; }

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns)
    {
        builder.Services
            .AddTransient<HealthHandler>()
            .AddSingleton<IFileSystem, FileSystem>();
    }

    /// <inheritdoc/>
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns)
    {
        app.MapGet<string>(StatusEndpoint, StatusName, StatusDescription, (HealthHandler handler) => handler.GetStatus());
        app.MapGet<string>(VersionEndpoint, VersionName, VersionDescription, async (HealthHandler handler) => await handler.GetVersionAsync());
    }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class HealthHandlerAddInExtensions
{
    /// <summary>
    /// Use a health handler endpoint.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add HealthHandler to.</param>
    /// <param name="statusEndpoint">The route for the status endpoint.</param>
    /// <param name="statusName">The status endpoint name.</param>
    /// <param name="statusDescription">A detailed description of the status endpoint.</param>
    /// <param name="versionEndpoint">The route for the version endpoint.</param>
    /// <param name="versionName">The version endpoint name.</param>
    /// <param name="versionDescription">A detailed description of the version endpoint.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithHealthHandler(
        this ApiBuilder apiBuilder,
        string statusEndpoint = "/health/status",
        string statusName = "GetHealthStatus",
        string statusDescription = "Check API health",
        string versionEndpoint = "/health/version",
        string versionName = "GetVersion",
        string versionDescription = "Get the API version")
    {
        apiBuilder.RegisterAddIn(new AddInHealthHandler
        {
            StatusEndpoint = statusEndpoint,
            StatusName = statusName,
            StatusDescription = statusDescription,
            VersionEndpoint = versionEndpoint,
            VersionName = versionName,
            VersionDescription = versionDescription
        });

        return apiBuilder;
    }
}
