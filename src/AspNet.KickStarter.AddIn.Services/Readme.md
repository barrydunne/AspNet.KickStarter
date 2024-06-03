# AspNet.KickStarter.AddIn.Services

This library provides a simple extension method to give a place to perform any custom service registration with the WebApplicationBuilder.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithServices(builder => builder.Services
        .AddTransient<IMyService, MyService>())
    .Build(args)
    .RunAsync();
```
