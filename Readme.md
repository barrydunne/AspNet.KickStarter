# AspNet.KickStarter

This library provides small helpers to reduce the repetitive code used to run my AspNet Minimal API projects.

### ApiBuilder

This simplifies the bootstrapping code to run a minimal API with optional support for Serilog, FluentValidation, Prometheus metrics and Swagger.

The most basic use in a Program.cs file would be
```
new ApiBuilder()
    .Build(args)
    .Run();
```

The following fluent extension methods are available to add extra functionality. Each extension may provide additional configuration parameters.
```
new ApiBuilder()
    .WithSerilog()
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(builder => {...})
    .WithEndpoints(app => {...})
    .WithMappings(() => {...})
    .WithMetrics()
    .WithFluentValidationFromAssemblyContaining<T>()
    .WithAdditionalConfiguration(builder => {...})
    .Build(args)
    .Run();
```

### HealthHandler

Provides basic health checks - status and version.
This can be added to the API using the `WithHealthHandler()` method.
```
new ApiBuilder()
    .WithHealthHandler()
    .Build(args)
    .Run();
```
By default this will respond on the following endpoints.
```
/health/status
/health/version
```
These endpoints may be configured passing `WithHealthHandler` arguments.

### IEndpointRouteBuilder Extensions

These extensions consolidate the AspNet extensions
```
app.MapXXX(route, handler)
   .WithName(name)
   .WithDescription(description)
   .WithOpenApi()
```
into a single extension with parameters for the name and description.
```
MapXXX(route, name, description, handler)
```

### Sample usage
 *Program.cs*

```
using AspNet.KickStarter;
using AspNet.KickStarter.Demo.HttpHandlers;
using AspNet.KickStarter.Demo.Models;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}")) // Optional Serilog diagnostic self logging action
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(RegisterServices)
    .WithEndpoints(MapEndpoints)
    .WithMetrics(8081)
    .Build(args)
    .Run();

void RegisterServices(WebApplicationBuilder builder)
{
    // HTTP Handlers
    builder.Services
        .AddTransient<NumberHandler>();
}

void MapEndpoints(WebApplication app)
{
    app.MapGet<GetDoubleResponse>("/double/{value}", "GetDouble", "Multiply by 2.",
        async (NumberHandler handler, double value)
            => await handler.GetDoubleAsync(value));
}
```

This will use the Serilog configuration in `appsettings.json`. For example:

*appsettings.json*

```
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console"
      },
      {
        "Name": "File",
        "Args": {
          "path": "My.Api_.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        },
        "MinimumLevel": "Information"
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://localhost:5341"
        }
      }
    ],
    "Enrich": [ "FromLogContext" ],
    "Properties": {
      "Application": "My API"
    }
  }
}
```


## AspNet.KickStarter.CQRS

This library provides the following basic interfaces used to implement CQRS with MediatR

* `ICommand` and `ICommandHandler`
* `IQuery` and `IQueryHandler`

These commands and queries rely on `Result`, `Result<T>` and `Error` types from this library, these types support implicit conversions for ease of use.

The `Result` class provides `Switch` and `Match` methods to conditionally perform actions depending on success or error of the result. They also provide access to the `Value` or `Error` values.

The `Error` class supports FluentValidation errors and provides an `AsHttpResult` method to produce a suitable IResult from a HTTP handler. For example:

```
var result = await _mediator.Send(new GetSomeQuery());
return result.Match(
    success => Results.Ok(success),
    error => error.AsHttpResult());
```

If the unsuccessful result contains a `ValidationResult` then this will return `Results.ValidationProblem` with the details, otherwise `Results.Problem`

The library also provides a generic `ValidationPipelineBehavior` class that enables use of FluentValidation for any commands or queries with a corresponding validator class. This can be registered as follows:

```
builder.Services
    .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
```

### Sample Usage

```
public record GetDoubleQuery(double Value) : IQuery<double>;
```
```
internal async Task<IResult> GetDoubleAsync(double value)
{
    var result = await _mediator.Send(new GetDoubleQuery(value));
    return result.Match(
        success => Results.Ok(success),
        error => error.AsHttpResult());
}
```
```
internal class GetDoubleQueryValidator : AbstractValidator<GetDoubleQuery>
{
    public GetDoubleQueryValidator()
    {
        RuleFor(_ => _.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);
    }
}
```
```
internal class GetDoubleQueryHandler : IQueryHandler<GetDoubleQuery, double>
{
    public Task<Result<double>> Handle(GetDoubleQuery request, CancellationToken cancellationToken)
    {
        Result<double> result;
        try
        {
            result = 2 * request.Value;
        }
        catch (Exception ex)
        {
            result = ex;
        }
        return Task.FromResult(result);
    }
}
```