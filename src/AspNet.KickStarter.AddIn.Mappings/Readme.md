# AspNet.KickStarter.AddIn.Mappings

This library provides an extension that giving a placeholder for configuring mappings with a package such as `AutoMapper` or `Mapster`.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithMappings(Mappings.Map)
    .Build(args)
    .RunAsync();
```

*Mappings.cs*
```csharp
internal static class Mappings
{
    internal static void Map()
    {
        TypeAdapterConfig<SomeEvent, SomeCommand>.NewConfig()
            .Map(dest => dest.JobId, src => src.Id)
            .Map(dest => dest.Email, src => src.UserEmail);
    }
}
```
