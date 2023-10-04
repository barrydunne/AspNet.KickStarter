# AspNet.KickStarter

These libraries provide small helpers to reduce the repetitive  code used to run my AspNet Minimal API projects.

## AspNet.KickStarter

This library provides the following helpers

### ApiBuilder

This simplifies the bootstrapping code to run a minimal API that with Serilog and Swagger.

### HealthHandler

Provides basic health checks - status and version.

### IEndpointRouteBuilder Extensions

These extensions simply consolidate the AspNet extensions
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
using AspNet.KickStarter.HttpHandlers;
using System.IO.Abstractions;

new ApiBuilder()
    .WithSerilog(msg => Console.WriteLine($"Serilog: {msg}")) // Optional Serilog diagnostic self logging action
    .WithSwagger()
    .WithServices(RegisterServices)
    .WithEndpoints(MapEndpointsp)
    .Build(args)
    .Run();

void RegisterServices(WebApplicationBuilder builder)
    => builder.Services
        .AddTransient<HealthHandler>()
        .AddSingleton<IFileSystem, FileSystem>();

void MapEndpointsp(WebApplication app)
{
    app.MapGet("/health/status", "GetHealthStatus", "Check API health",
        (HealthHandler handler) => handler.GetStatus());
    app.MapGet("/health/version", "GetVersion", "Get the API version",
        async (HealthHandler handler) => await handler.GetVersionAsync());
}
```

This will use the Serilog configuration in `appsettings.json`. For example:

*appsettings.json*

```
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Debug", "Serilog.Sinks.Console", "Serilog.Sinks.File", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Debug"
      },
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


## AspNet.KickStarter.CQRS.Abstractions

This library provides the following basic interfaces used to implement CQRS with MediatR

* ICommand
* IQuery

Along with handler interfaces

* ICommandHandler
* IQueryHandler

### Sample Usage

```
public class GetVersionQuery : IQuery<string> { }

internal class GetVersionQueryHandler : IQueryHandler<GetVersionQuery, string>
{
    public Task<string> Handle(GetVersionQuery query, CancellationToken cancellationToken) => Task.FromResult("1.0");
}


public class SetValueCommand : ICommand
{
    public string Value { get; }
    public SetValueCommand(string value) => Value = value;
}

internal class SetValueCommandHandler : ICommandHandler<SetValueCommand>
{
    public Task<Result> Handle(SetValueCommand command, CancellationToken cancellationToken)
    {
        try
        {
            // Set the value to command.Value
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure(ex.Message);
        }
    }
}
```