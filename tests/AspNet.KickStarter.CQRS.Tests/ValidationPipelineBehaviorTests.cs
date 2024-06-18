using AspNet.KickStarter.FunctionalResult;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "ValidationPipelineBehavior")]
internal class ValidationPipelineBehaviorTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public async Task ValidationPipelineBehavior_without_validator_returns_next()
    {
        var mockLogger = Substitute.For<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockLogger);

        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);

        Assert.That(result.Value, Is.EqualTo(nextValue));
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }

    [Test]
    public async Task ValidationPipelineBehavior_with_success_validator_returns_next()
    {
        var validationResult = new ValidationResult();
        var mockValidator = Substitute.For<IValidator<TestQuery>>();
        mockValidator.ValidateAsync(Arg.Any<TestQuery>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);
        var mockLogger = Substitute.For<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockValidator, mockLogger);

        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);

        Assert.That(result.Value, Is.EqualTo(nextValue));
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }

    [Test]
    public async Task ValidationPipelineBehavior_with_error_validator_returns_error_Result()
    {
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") });
        var mockValidator = Substitute.For<IValidator<TestQuery>>();
        mockValidator.ValidateAsync(Arg.Any<TestQuery>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);
        var mockLogger = Substitute.For<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockValidator, mockLogger);

        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);

        Assert.That(result.Error?.ValidationResult, Is.EqualTo(validationResult));
        Task<Result<int>> Next() => throw new NotImplementedException();
    }
}
