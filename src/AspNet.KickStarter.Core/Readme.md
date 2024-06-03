# AspNet.KickStarter.Core

This library provides small helpers to reduce the repetitive code used to run AspNet Minimal API projects.

### ApiBuilder

This simplifies the bootstrapping code to run a minimal API with optional support for Serilog, FluentValidation, Prometheus metrics and Swagger.

The most basic use in a Program.cs file would be
```csharp
await new ApiBuilder().Build(args).RunAsync();
```

Many fluent extension methods are available through the AddIn packages to add extra functionality. Each extension may provide additional configuration parameters.

Further information about the extensions may be found in the readme for the AddIn package. See [NuGet](https://www.nuget.org/packages?q=AspNet.KickStarter) for the full list of AddIns.

```csharp
await new ApiBuilder()
    .WithSerilog()
    .WithSwagger()
    .WithHealthHandler()
    .WithServices(builder => {...})
    .WithEndpoints(app => {...})
    .WithMappings(() => {...})
    .WithOpenTelemetry()
    .WithFluentValidationFromAssemblyContaining<T>()
    .WithAdditionalConfiguration(builder => {...})
    .Build(args)
    .RunAsync();
```


### IEndpointRouteBuilder Extensions

These extensions consolidate the AspNet extensions
```csharp
app.MapXXX(route, handler)
   .WithName(name)
   .WithDescription(description)
   .WithOpenApi()
```
into a single extension with parameters for the name and description.
```csharp
MapXXX(route, name, description, handler)
```

### IMeterFactory Extensions

This allows a Meter to be created with the Assembly name
```csharp
var meter = meterFactory.CreateAssemblyMeter();
```