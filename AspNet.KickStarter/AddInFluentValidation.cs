using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the FluentValidation add-in.
/// </summary>
internal class AddInFluentValidation : IAddIn
{
    /// <summary>
    /// Gets or sets a type whose assembly to scan for validators.
    /// </summary>
    internal required Type Type { get; set; }

    /// <summary>
    /// Gets or sets the lifetime of the validators.
    /// </summary>
    internal required ServiceLifetime Lifetime { get; set; }

    /// <summary>
    /// Gets or sets an optional filter that allows certain types to be skipped from registration.
    /// </summary>
    internal Func<AssemblyScanner.AssemblyScanResult, bool>? Filter { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to include internal validators.
    /// </summary>
    internal required bool IncludeInternalTypes { get; set; } = false;

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder)
        => builder.Services.AddValidatorsFromAssemblyContaining(Type, Lifetime, Filter, IncludeInternalTypes);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app) { }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class FluentValidationAddInExtensions
{
    /// <summary>
    /// Use FluentValidation.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add FluentValidation to.</param>
    /// <typeparam name="TValidator">A validator that is from the assembly to scan.</typeparam>
    /// <param name="lifetime">The lifetime of the validators. The default is scoped (per-request in web applications).</param>
    /// <param name="filter">Optional filter that allows certain types to be skipped from registration.</param>
    /// <param name="includeInternalTypes">Include internal validators. The default is false.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithFluentValidationFromAssemblyContaining<TValidator>(this ApiBuilder apiBuilder, ServiceLifetime lifetime = ServiceLifetime.Scoped, Func<AssemblyScanner.AssemblyScanResult, bool>? filter = null, bool includeInternalTypes = true)
    {
        apiBuilder.RegisterAddIn(new AddInFluentValidation
        {
            Type = typeof(TValidator),
            Lifetime = lifetime,
            Filter = filter,
            IncludeInternalTypes = includeInternalTypes
        });

        return apiBuilder;
    }
}
