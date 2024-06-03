# AspNet.KickStarter.AddIn.Endpoints

This library provides an extension intended to be used to configure the minimal API endpoint mappings.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithEndpoints(MapEndpoints)
    .Build(args)
    .RunAsync();

void MapEndpoints(WebApplication app)
{
    app.MapGet<GetDoubleResponse>("/double/{value}", "GetDouble", "Multiply by 2.",
        async (NumberHandler handler, double value)
            => await handler.GetDoubleAsync(value));
}
```
