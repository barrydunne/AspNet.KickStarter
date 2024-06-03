using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Services add-in.
/// </summary>
internal class AddInServices : IAddIn
{
    /// <summary>
    /// Gets or sets the action that will register the services for the API.
    /// </summary>
    internal required Action<WebApplicationBuilder> RegisterServices { get; set; }

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns) => RegisterServices.Invoke(builder);

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns) { }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class ServicesAddInExtensions
{
    /// <summary>
    /// Provide the action that will register the services for the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Serilog to.</param>
    /// <param name="registerServices">The action that will register the services for the API.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithServices(this ApiBuilder apiBuilder, Action<WebApplicationBuilder> registerServices)
    {
        apiBuilder.RegisterAddIn(new AddInServices
        {
            RegisterServices = registerServices
        });

        return apiBuilder;
    }
}
