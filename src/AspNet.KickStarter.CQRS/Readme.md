# AspNet.KickStarter.CQRS

This library provides the following basic interfaces used to implement CQRS with MediatR

* `ICommand` and `ICommandHandler`
* `IQuery` and `IQueryHandler`

These commands and queries rely on `Result`, `Result<T>` and `Error` types from the `AspNet.KickStarter.FunctionalResult` library.

## TracePipelineBehavior

The library provides a generic `TracePipelineBehavior` class that adds automatic trace activities for any commands or queries. This can be registered as follows:

```csharp
builder.Services
    .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddTracePipelineBehavior();
```

## ValidationPipelineBehavior

The library provides a generic `ValidationPipelineBehavior` class that enables use of FluentValidation for any commands or queries with a corresponding validator class. This can be registered as follows:

```csharp
builder.Services
    .AddMediatR(_ => _.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()))
    .AddValidationPipelineBehavior()
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