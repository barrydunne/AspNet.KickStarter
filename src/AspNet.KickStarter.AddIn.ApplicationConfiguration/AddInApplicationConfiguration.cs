using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the ApplicationConfiguration add-in.
/// </summary>
internal class AddInApplicationConfiguration : IAddIn
{
    /// <summary>
    /// Gets or sets the action that will perform custom configuration for the API.
    /// </summary>
    internal required Action<WebApplication> ApplicationConfiguration { get; set; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns) { }

    /// <inheritdoc/>
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns) => ApplicationConfiguration.Invoke(app);
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class ApplicationConfigurationAddInExtensions
{
    /// <summary>
    /// Provide the action that will perform custom configuration for the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add ApplicationConfiguration to.</param>
    /// <param name="applicationConfiguration">The action that will perform custom configuration for the API.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithApplicationConfiguration(this ApiBuilder apiBuilder, Action<WebApplication> applicationConfiguration)
    {
        apiBuilder.RegisterAddIn(new AddInApplicationConfiguration
        {
            ApplicationConfiguration = applicationConfiguration
        });

        return apiBuilder;
    }
}
