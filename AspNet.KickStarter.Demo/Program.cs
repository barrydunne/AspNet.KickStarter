using AspNet.KickStarter;
using AspNet.KickStarter.HttpHandlers;
using System.IO.Abstractions;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}")) // Optional Serilog diagnostic self logging action
    .WithSwagger()
    .WithServices(RegisterServices)
    .WithEndpoints(MapEndpointsp)
    .WithMetrics(8081)
    .Build(args)
    .Run();

void RegisterServices(WebApplicationBuilder builder)
{
    // API Handlers
    builder.Services
        .AddTransient<HealthHandler>();

    // FileSystem
    builder.Services
        .AddSingleton<IFileSystem, FileSystem>();
}

void MapEndpointsp(WebApplication app)
{
    app.MapGet("/health/status", "GetHealthStatus", "Check API health",
        (HealthHandler handler) => handler.GetStatus());
    app.MapGet("/health/version", "GetVersion", "Get the API version",
        async (HealthHandler handler) => await handler.GetVersionAsync());
}
