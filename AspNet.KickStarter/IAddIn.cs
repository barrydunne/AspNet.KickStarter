using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter;

/// <summary>
/// Additional functionality that can be configured in the application.
/// </summary>
internal interface IAddIn
{
    /// <summary>
    /// Configure the add-in with the builder.
    /// </summary>
    /// <param name="builder">The builder to configure.</param>
    void Configure(WebApplicationBuilder builder);

    /// <summary>
    /// Configure the add-in with the application.
    /// </summary>
    /// <param name="app">The application to configure.</param>
    void Configure(WebApplication app);
}
