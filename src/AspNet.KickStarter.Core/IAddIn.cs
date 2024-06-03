using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter;

/// <summary>
/// Additional functionality that can be configured in the application.
/// </summary>
public interface IAddIn
{
    /// <summary>
    /// Configure the add-in with the builder.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    /// <param name="addIns">The full collection of AddIs that may be used to ensure compatibility.</param>
    void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns);

    /// <summary>
    /// Configure the add-in with the application.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    /// <param name="addIns">The full collection of AddIs that may be used to ensure compatibility.</param>
    void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns);
}
