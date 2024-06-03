using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Swagger add-in.
/// </summary>
internal class AddInSwagger : IAddIn
{
    /// <summary>
    /// Gets or sets a value indicating whether Bearer tokens are supported.
    /// </summary>
    internal bool WithSwaggerBearerToken { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to only include swagger page when running in development mode.
    /// </summary>
    internal bool WithSwaggerOnlyInDevelopment { get; set; } = false;

    /// <inheritdoc/>
    public void Configure(WebApplicationBuilder builder, IReadOnlyCollection<IAddIn> addIns)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options => SetupSwaggerGenOptions(options));
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns)
    {
        if (WithSwaggerOnlyInDevelopment && !app.Environment.IsDevelopment())
            return;

        app.UseSwagger(swagger => swagger.RouteTemplate = "{documentname}/swagger.json");
        app.UseSwaggerUI(ui =>
        {
            ui.RoutePrefix = "";
            ui.EnableTryItOutByDefault();
        });
    }

    /// <summary>
    /// Configure Bearer token authorization if required.
    /// </summary>
    /// <param name="options">The Swagger options to configure.</param>
    internal void SetupSwaggerGenOptions(SwaggerGenOptions options)
    {
        if (!WithSwaggerBearerToken)
            return;

        var scheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter your access token below without the 'Bearer ' prefix.",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http, // Does not require the "Bearer " prefix to be used, unlike ApiKey.
            Scheme = "Bearer"
        };
        options.AddSecurityDefinition("Bearer", scheme);
        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}

/// <summary>
/// Provides the extension method to register the add-in.
/// </summary>
public static class SwaggerAddInExtensions
{
    /// <summary>
    /// Use Swagger/OpenApi in the API.
    /// </summary>
    /// <param name="apiBuilder">The <see cref="ApiBuilder"/> to add Swagger to.</param>
    /// <param name="onlyInDevelopment">Whether to only include swagger page when running in development mode.</param>
    /// <param name="useBearerToken">Whether to include Bearer token authorization.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithSwagger(this ApiBuilder apiBuilder, bool onlyInDevelopment = false, bool useBearerToken = false)
    {
        apiBuilder.RegisterAddIn(new AddInSwagger
        {
            WithSwaggerOnlyInDevelopment = onlyInDevelopment,
            WithSwaggerBearerToken = useBearerToken
        });

        return apiBuilder;
    }
}
