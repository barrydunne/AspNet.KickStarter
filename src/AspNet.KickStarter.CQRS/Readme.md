# AspNet.KickStarter.CQRS

This library provides the following basic interfaces used to implement CQRS with MediatR

* `ICommand` and `ICommandHandler`
* `IQuery` and `IQueryHandler`

These commands and queries rely on `Result`, `Result<T>` and `Error` types from this library, these types support implicit conversions for ease of use.

The `Result` class provides `Switch` and `Match` methods to conditionally perform actions depending on success or error of the result. They also provide access to the `Value` or `Error` values.

The `Error` class supports FluentValidation errors and provides an `AsHttpResult` method to produce a suitable IResult from a HTTP handler. For example:

```csharp
var result = await _mediator.Send(new GetSomeQuery());
return result.Match(
    success => Results.Ok(success),
    error => error.AsHttpResult());
```

If the unsuccessful result contains a `ValidationResult` then this will return `Results.ValidationProblem` with the details, otherwise `Results.Problem`

## OpenTelemetryTracePipelineBehavior

The library also provides a generic `OpenTelemetryTracePipelineBehavior` class that adds automatic trace activities for any commands or queries. This can be registered as follows:

```csharp
builder.Services
    .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(OpenTelemetryTracePipelineBehavior<,>));
```

## ValidationPipelineBehavior

The library also provides a generic `ValidationPipelineBehavior` class that enables use of FluentValidation for any commands or queries with a corresponding validator class. This can be registered as follows:

```csharp
builder.Services
    .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>))
    .AddValidatorsFromAssembly(Assembly.GetExecutingAssembly(), includeInternalTypes: true);
```

### Sample Usage

```csharp
public record GetDoubleQuery(double Value) : IQuery<double>;
```
```csharp
internal async Task<IResult> GetDoubleAsync(double value)
{
    var result = await _mediator.Send(new GetDoubleQuery(value));
    return result.Match(
        success => Results.Ok(success),
        error => error.AsHttpResult());
}
```
```csharp
internal class GetDoubleQueryValidator : AbstractValidator<GetDoubleQuery>
{
    public GetDoubleQueryValidator()
    {
        RuleFor(_ => _.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);
    }
}
```
```csharp
internal class GetDoubleQueryHandler : IQueryHandler<GetDoubleQuery, double>
{
    public Task<Result<double>> Handle(GetDoubleQuery request, CancellationToken cancellationToken)
    {
        Result<double> result;
        try
        {
            result = 2 * request.Value;
        }
        catch (Exception ex)
        {
            result = ex;
        }
        return Task.FromResult(result);
    }
}
```