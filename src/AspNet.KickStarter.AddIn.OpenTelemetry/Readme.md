# AspNet.KickStarter.AddIn.OpenTelemetry

This library provides an extension that configures `OpenTelemetry` for the application.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithOpenTelemetry()
    .Build(args)
    .RunAsync();
```

### Extended Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithOpenTelemetry(
        prometheusPort: 8081,
        configureTraceBuilder: builder => {...}
        configureMetricsBuilder: builder => {...}
        configureLoggerOptions: options => {...})
    .Build(args)
    .RunAsync();
```

*MyService.cs*
```csharp
public MyService(IMeterFactory meterFactory, ITraceActivity traceActivity)
{
    var meter = meterFactory.CreateAssemblyMeter();
    _traceActivity = traceActivity;

    var subjectName = nameof(MyService).ToLower();

    _count = meter.CreateCounter<long>($"{meter.Name.ToLower()}.{subjectName}.process.count", description: "The number of things processed.");
    
    _duration = meter.CreateHistogram<double>($"{meter.Name.ToLower()}.{subjectName}.process.duration", description: "Time taken to process a thing.", unit: "ms");
}

public Process(Thing thing)
{
    using var trace = _traceActivity.StartActivity("Thing Processing");
    _count.Add(1);
    var stopwatch = Stopwatch.StartNew();
    ...
    _duration.Record(stopwatch.ElapsedMilliseconds);
}
```

This will instrument the application for metrics and traces to be sent to a collector at the endpoint supplied in the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable.

Optionally it can run a listener on a separate port for `Prometheus` scraping.

A full example can be seen in the [AspNet.KickStarter.Demo.DockerCompose](https://github.com/barrydunne/AspNet.KickStarter/tree/main/demo/) project where the information is available in the Aspire dashboard as well as Grafana.