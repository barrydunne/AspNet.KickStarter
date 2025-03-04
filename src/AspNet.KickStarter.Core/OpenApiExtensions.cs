#if NET9_0_OR_GREATER

using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Provides replacement for extension method removed in v9 of Microsoft.AspNetCore.OpenApi.
/// </summary>
[ExcludeFromCodeCoverage(Justification = "Empty replacement for removed extension method.")]
internal static class OpenApiExtensions
{
    /// <summary>
    /// Adds an empty replacement for the WithOpenApi extension removed in v9 of Microsoft.AspNetCore.OpenApi.
    /// </summary>
    /// <param name="builder">The <see cref="IEndpointConventionBuilder"/>.</param>
    /// <returns>A <see cref="IEndpointConventionBuilder"/> that can be used to further customize the endpoint.</returns>
    /// <typeparam name="TBuilder">The type of the builder.</typeparam>
    public static TBuilder WithOpenApi<TBuilder>(this TBuilder builder) where TBuilder : IEndpointConventionBuilder
        => builder;
}

#endif
