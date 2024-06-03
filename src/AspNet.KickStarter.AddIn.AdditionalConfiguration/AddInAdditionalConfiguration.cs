using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the AdditionalConfiguration add-in.
/// </summary>
internal class AddInAdditionalConfiguration : IAddIn
{
    /// <summary>
    /// Gets or sets the action that will perform custom configuration for the API.
    /// </summary>
    internal required Action<WebApplicationBuilder> AdditionalConfiguration { get; set; }

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns) => AdditionalConfiguration.Invoke(builder);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns) { }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class AdditionalConfigurationAddInExtensions
{
    /// <summary>
    /// Provide the action that will perform custom configuration for the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add AdditionalConfiguration to.</param>
    /// <param name="additionalConfiguration">The action that will perform custom configuration for the API.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithAdditionalConfiguration(this ApiBuilder apiBuilder, Action<WebApplicationBuilder> additionalConfiguration)
    {
        apiBuilder.RegisterAddIn(new AddInAdditionalConfiguration
        {
            AdditionalConfiguration = additionalConfiguration
        });

        return apiBuilder;
    }
}
