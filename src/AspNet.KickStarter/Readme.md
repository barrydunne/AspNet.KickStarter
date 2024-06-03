# AspNet.KickStarter

This library provides small helpers to reduce the repetitive code used to run AspNet Minimal API projects.

The `AspNet.KickStarter` package can be used as a single reference to pull in all of the individual packages in the collection.

To reduce the dependencies the individual packages may be referenced instead.

* `AspNet.KickStarter.Core` This is the base library that provides the `ApiBuilder`.
* `AspNet.KickStarter.AddIn.xxx` These are the packages that provide the extended functionality such as OpenTelemetry and FluentValidation.
* `AspNet.KickStarter.CQRS` This provides a number of interfaces and classes to use when working with MediatR. Including functional result types and automatic trace activities of handlers.

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