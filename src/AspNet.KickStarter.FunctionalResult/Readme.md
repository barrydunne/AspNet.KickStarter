# AspNet.KickStarter.FunctionalResult

This library provides `Result`, `Result<T>` and `Error` types, these types support implicit conversions for ease of use.

The `Result` class provides `Switch`, `Match` and `Bind` methods to conditionally perform actions depending on success or error of the result. They also provide access to the `Value` or `Error` values.

The `Error` class supports FluentValidation errors and provides an `AsHttpResult` method to produce a suitable IResult from a HTTP handler. For example:

```csharp
var result = await _mediator.Send(new GetSomeQuery());
return result.Match(
    success => Results.Ok(success),
    error => error.AsHttpResult());
```

If the unsuccessful result contains a `ValidationResult` then this will return `Results.ValidationProblem` with the details, otherwise `Results.Problem`


### Sample Usage

```csharp
internal IResult GetDoubleAsync(double value)
{
    var result = doubleService.GetDouble(value);
    return result.Match(
        success => Results.Ok(success),
        error => error.AsHttpResult());
}
```
```csharp
internal class DoubleService
{
    public Result<double> GetDouble(double value)
    {
        try
        {
            return 2 * value;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
```