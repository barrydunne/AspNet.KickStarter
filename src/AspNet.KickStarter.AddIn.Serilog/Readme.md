# AspNet.KickStarter.AddIn.Serilog

This library provides an extension that configures `Serilog` logging for the application using the sink configuration from the appsettings.json.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithSerilog()
    .Build(args)
    .RunAsync();
```

*appsettings.json*
```json
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
