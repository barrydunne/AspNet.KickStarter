# AspNet.KickStarter.AddIn.AdditionalConfiguration

This library provides a simple extension method to give a place to perform any custom configuration of the WebApplicationBuilder that does not fit into any of the other extension methods.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithAdditionalConfiguration(_ => _.Services
        .Configure<IdentityServerOptions>(_ => _.IssuerUri = "http://ids.example.com:8080")
        .AddIdentityServer()
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddDeveloperSigningCredential())
    .WithApplicationConfiguration(_ => _.UseIdentityServer())
    .Build(args)
    .RunAsync();
```
