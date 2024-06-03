# AspNet.KickStarter.AddIn.FluentValidation

This library provides an extension intended to be used to configure `FluentValidation`.

### Sample Usage

*Program.cs*
```csharp
await new ApiBuilder()
    .WithEndpoints(MapEndpoints)
    .WithFluentValidationFromAssemblyContaining<CreateJobRequestValidator>()
    .Build(args)
    .RunAsync();

void MapEndpoints(WebApplication app)
{
    app.MapPost<CreateJobResponse>("/job", "CreateJob", "Create a new job.",
            async (JobHandler handler,
                   [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Disallow)] CreateJobRequest request,
                   IValidator<CreateJobRequest> validator)
                => await handler.CreateJobAsync(request, validator));
}
```

*JobHandler.cs*
```csharp
internal async Task<IResult> CreateJobAsync(CreateJobRequest request, IValidator<CreateJobRequest> validator)
{
    var validationResult = await validator.ValidateAsync(request);
    if (!validationResult.IsValid)
        return Results.ValidationProblem(validationResult.ToDictionary());
    ...
}
```

 *CreateJobRequestValidator.cs*
```csharp
internal class CreateJobRequestValidator : AbstractValidator<CreateJobRequest>
{
    public CreateJobRequestValidator()
    {
        RuleFor(_ => _.Email)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(128);
    }
}
```
