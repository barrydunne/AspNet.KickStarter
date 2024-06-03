# AspNet.KickStarter.AddIn.HealthHandler

This library provides an extension that implements basic health check and version endpoints.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithHealthHandler()
    .Build(args)
    .RunAsync();
```



### HealthHandler

By default this will respond on the following endpoints.
```csharp
/health/status
/health/version
```
These endpoints may be configured passing arguments to the `WithHealthHandler` extension.

The version returned may be configured by storing a text file in the app path, or if unavailable the assembly version will be returned.