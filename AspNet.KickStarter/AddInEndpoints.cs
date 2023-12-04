using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Endpoints add-in.
/// </summary>
internal class AddInEndpoints : IAddIn
{
    /// <summary>
    /// Gets or sets the action that will map the endpoints for the API.
    /// </summary>
    internal required Action<WebApplication> Endpoints { get; set; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplicationBuilder builder) { }

    /// <inheritdoc/>
    public void Configure(WebApplication app) => Endpoints.Invoke(app);
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class EndpointsAddInExtensions
{
    /// <summary>
    /// Provide the action that will map the endpoints for the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Endpoints to.</param>
    /// <param name="mapEndpoints">The action that will map the endpoints for the API.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithEndpoints(this ApiBuilder apiBuilder, Action<WebApplication> mapEndpoints)
    {
        apiBuilder.RegisterAddIn(new AddInEndpoints
        {
            Endpoints = mapEndpoints
        });

        return apiBuilder;
    }
}
