using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Mappings add-in.
/// </summary>
internal class AddInMappings : IAddIn
{
    /// <summary>
    /// Gets or sets the action that will configure the type mappings.
    /// </summary>
    internal required Action Mapper { get; set; }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplicationBuilder builder) { }

    /// <inheritdoc/>
    public void Configure(WebApplication app) => Mapper.Invoke();
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class MappingsAddInExtensions
{
    /// <summary>
    /// Provide the action that will configure the type mappings.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add mappings to.</param>
    /// <param name="mapper">The action that will configure the type mappings.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithMappings(this ApiBuilder apiBuilder, Action mapper)
    {
        apiBuilder.RegisterAddIn(new AddInMappings
        {
            Mapper = mapper
        });

        return apiBuilder;
    }
}
