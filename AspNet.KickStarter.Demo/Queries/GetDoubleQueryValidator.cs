using FluentValidation;

namespace AspNet.KickStarter.Demo.Queries;

internal class GetDoubleQueryValidator : AbstractValidator<GetDoubleQuery>
{
    public GetDoubleQueryValidator()
    {
        RuleFor(_ => _.Value)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(10);
    }
}
