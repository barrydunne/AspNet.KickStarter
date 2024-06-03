# AspNet.KickStarter.Tracing

This library provides an ITraceActivity that is registered by `WithOpenTelemetry` and may be used to add tracing to operations.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithOpenTelemetry()
    .Build(args)
    .RunAsync();
```

*MyService.cs*
```csharp
public MyService(ITraceActivity traceActivity) => _traceActivity = traceActivity;

public Process(Thing thing)
{
    using var trace = _traceActivity.StartActivity("Thing Processing");
    ...
}
```
