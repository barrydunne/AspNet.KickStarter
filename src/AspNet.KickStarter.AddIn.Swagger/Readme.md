# AspNet.KickStarter.AddIn.Swagger

This library provides an extension method to include swagger in the application.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithSwagger()
    .Build(args)
    .RunAsync();
```

### Extended Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithSwagger(
        onlyInDevelopment: true,
        useBearerToken: true
    )
    .Build(args)
    .RunAsync();
```
