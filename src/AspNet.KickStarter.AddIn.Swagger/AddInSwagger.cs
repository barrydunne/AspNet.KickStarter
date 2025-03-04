using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NSwag;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using System.Diagnostics.CodeAnalysis;

namespace AspNet.KickStarter;

/// <summary>
/// Configuration of the Swagger add-in.
/// </summary>
internal class AddInSwagger : IAddIn
{
    /// <summary>
    /// Gets or sets the document title.
    /// </summary>
    internal string Title { get; set; } = "API Documentation";

    /// <summary>
    /// Gets or sets the path to the swagger UI page.
    /// </summary>
    public string Path { get; set; } = "";

    /// <summary>
    /// Gets or sets the path to the ReDoc documentation page.
    /// </summary>
    public string ReDocPath { get; set; } = "/docs";

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
        builder.Services.AddSwaggerDocument(SetupSwaggerGenOptions);
#if NET9_0_OR_GREATER
        builder.Services.AddOpenApi();
#endif
    }

    /// <inheritdoc/>
    [ExcludeFromCodeCoverage]
    public void Configure(WebApplication app, IReadOnlyCollection<IAddIn> addIns)
    {
        if (WithSwaggerOnlyInDevelopment && !app.Environment.IsDevelopment())
            return;

#if NET9_0_OR_GREATER
        app.MapOpenApi("/swagger/swagger.json");
#else
        app.UseOpenApi(configure => configure.Path = "/swagger/swagger.json");
#endif

        app
            .UseSwaggerUi(configure =>
            {
                configure.Path = Path;
                configure.DocumentPath = $"/swagger/swagger.json";
                configure.DocumentTitle = Title;
                configure.DocExpansion = "list";
                configure.DefaultModelsExpandDepth = -1;
                configure.AdditionalSettings["tryItOutEnabled"] = "true";
                configure.CustomInlineStyles = WithSwaggerBearerToken
                    ? ".schemes-server-container {display: none !important}"
                    : ".scheme-container {display: none !important}";
            })
            .UseReDoc(configure =>
            {
                configure.Path = ReDocPath;
                configure.DocumentPath = $"/swagger/swagger.json";
                configure.DocumentTitle = Title;
                configure.AdditionalSettings["pathInMiddlePanel"] = "true";
                configure.AdditionalSettings["jsonSampleExpandLevel"] = "2";
            })
            .Use((context, next) =>
            {
                if (context.Request.Path.Value?.EndsWith("/swagger.json") == true)
                    context.Response.Headers["Access-Control-Allow-Origin"] = "*";
                return next.Invoke();
            });
    }

    /// <summary>
    /// Configure Bearer token authorization if required.
    /// </summary>
    /// <param name="settings">The Swagger settings to configure.</param>
    internal void SetupSwaggerGenOptions(AspNetCoreOpenApiDocumentGeneratorSettings settings)
    {
        settings.Title = Title;
        if (!WithSwaggerBearerToken)
            return;

        var scheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter 'Bearer {your token}' to authenticate.",
            In = OpenApiSecurityApiKeyLocation.Header,
            Type = OpenApiSecuritySchemeType.ApiKey,
            Scheme = "bearer"
        };

        settings.AddSecurity("Bearer", [], scheme);
        settings.OperationProcessors.Add(new OperationSecurityScopeProcessor("Bearer"));
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
    /// <param name="title">The document title.</param>
    /// <param name="path">The path to the swagger UI page.</param>
    /// <param name="redocPath">The path to the ReDoc documentation page.</param>
    /// <param name="onlyInDevelopment">Whether to only include swagger page when running in development mode.</param>
    /// <param name="useBearerToken">Whether to include Bearer token authorization.</param>
    /// <returns>The current builder.</returns>
    public static ApiBuilder WithSwagger(
        this ApiBuilder apiBuilder,
        string title = "API Documentation",
        string path = "",
        string redocPath = "/docs",
        bool onlyInDevelopment = false,
        bool useBearerToken = false)
    {
        apiBuilder.RegisterAddIn(new AddInSwagger
        {
            Title = title,
            Path = path,
            ReDocPath = redocPath,
            WithSwaggerOnlyInDevelopment = onlyInDevelopment,
            WithSwaggerBearerToken = useBearerToken
        });

        return apiBuilder;
    }
}
