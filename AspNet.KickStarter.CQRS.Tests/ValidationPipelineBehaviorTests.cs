using AspNet.KickStarter.CQRS.Abstractions.Queries;
using AutoFixture;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "ValidationPipelineBehavior")]
internal class ValidationPipelineBehaviorTests
{
    private readonly Fixture _fixture;

    public ValidationPipelineBehaviorTests() => _fixture = new Fixture();

    [Test]
    public async Task ValidationPipelineBehavior_without_validator_returns_next()
    {
        var mockLogger = new Mock<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockLogger.Object);
        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);
        Assert.That(result.Value, Is.EqualTo(nextValue));
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }

    [Test]
    public async Task ValidationPipelineBehavior_with_success_validator_returns_next()
    {
        var validationResult = new ValidationResult();
        var mockValidator = new Mock<IValidator<TestQuery>>();
        mockValidator.Setup(_ => _.ValidateAsync(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestQuery _, CancellationToken _) => validationResult);

        var mockLogger = new Mock<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockValidator.Object, mockLogger.Object);
        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);
        Assert.That(result.Value, Is.EqualTo(nextValue));
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }

    [Test]
    public async Task ValidationPipelineBehavior_with_error_validator_returns_error_Result()
    {
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Property", "Error") });
        var mockValidator = new Mock<IValidator<TestQuery>>();
        mockValidator.Setup(_ => _.ValidateAsync(It.IsAny<TestQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TestQuery _, CancellationToken _) => validationResult);

        var mockLogger = new Mock<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new ValidationPipelineBehavior<TestQuery, Result<int>>(mockValidator.Object, mockLogger.Object);
        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);
        Assert.That(result.Error?.ValidationResult, Is.EqualTo(validationResult));
        Task<Result<int>> Next() => throw new NotImplementedException();
    }
}

public record TestQuery : IQuery<int>;
