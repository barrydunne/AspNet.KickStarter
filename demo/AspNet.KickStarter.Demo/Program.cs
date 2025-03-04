using AspNet.KickStarter;
using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.Demo.HttpHandlers;
using AspNet.KickStarter.Demo.Models;
using AspNet.KickStarter.Demo.Queries;
using FluentValidation;
using Mapster;
using Serilog;
using System.Reflection;

await new ApiBuilder()
    .WithSerilog(
        // Optional serilog customizations
        configure: config => config.Enrich.WithEnvironmentName(),
        debugOutput: message => Console.WriteLine($"Serilog: {message}"))
    .WithSwagger(title: "Demo API")
    .WithHealthHandler()
    .WithServices(RegisterServices)
    .WithEndpoints(MapEndpoints)
    .WithMappings(MapTypes)
    .WithOpenTelemetry(8081)
    .Build(args)
    .RunAsync();

void RegisterServices(WebApplicationBuilder builder)
{
    // HTTP Handlers
    builder.Services
        .AddTransient<NumberHandler>();

    // CQRS
    builder.Services
        .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
        .AddTracePipelineBehavior()
        .AddValidationPipelineBehavior()
        .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
}

void MapEndpoints(WebApplication app)
{
    app.MapGet<GetDoubleResponse>("/number/double/{value}", "GetDouble", "Get a number doubled. Must be between 0 and 10 inclusive.",
        async (NumberHandler handler, double value)
            => await handler.GetDoubleAsync(new GetDoubleRequest { Value = value }));
}

void MapTypes()
{
    TypeAdapterConfig<GetDoubleRequest, GetDoubleQuery>.NewConfig()
        .Map(dest => dest.Value, src => src.Value);

    TypeAdapterConfig<double, GetDoubleResponse>.NewConfig()
        .Map(dest => dest.Value, src => src);
}
