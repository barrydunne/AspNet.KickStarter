using AutoFixture;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "Error")]
internal class ErrorTests
{
    private readonly Fixture _fixture;

    public ErrorTests() => _fixture = new Fixture();

    [Test]
    public void Error_constructed_from_string_sets_Message()
    {
        var message = _fixture.Create<string>();
        var sut = new Error(message);
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_constructed_from_string_has_no_InnerException()
    {
        var message = _fixture.Create<string>();
        var sut = new Error(message);
        Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void Error_constructed_from_string_has_no_ValidationResult()
    {
        var message = _fixture.Create<string>();
        var sut = new Error(message);
        Assert.That(sut.ValidationResult, Is.Null);
    }

    [Test]
    public void Error_constructed_from_Exception_sets_Message()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        var sut = new Error(exception);
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_constructed_from_Exception_sets_InnerException()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        var sut = new Error(exception);
        Assert.That(sut.InnerException, Is.EqualTo(exception));
    }

    [Test]
    public void Error_constructed_from_Exception_has_no_ValidationResult()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        var sut = new Error(exception);
        Assert.That(sut.ValidationResult, Is.Null);
    }

    [Test]
    public void Error_constructed_from_ValidationResult_sets_Message()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        var message = validationResult.ToString();
        var sut = new Error(validationResult);
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_constructed_from_ValidationResult_has_no_InnerException()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        var sut = new Error(validationResult);
        Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void Error_constructed_from_ValidationResult_sets_ValidationResult()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        var sut = new Error(validationResult);
        Assert.That(sut.ValidationResult, Is.EqualTo(validationResult));
    }

    [Test]
    public void Error_converted_from_string_sets_Message()
    {
        var message = _fixture.Create<string>();
        Error sut = message;
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_converted_from_string_has_no_InnerException()
    {
        var message = _fixture.Create<string>();
        Error sut = message;
        Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void Error_converted_from_string_has_no_ValidationResult()
    {
        var message = _fixture.Create<string>();
        Error sut = message;
        Assert.That(sut.ValidationResult, Is.Null);
    }

    [Test]
    public void Error_converted_from_Exception_sets_Message()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        Error sut = exception;
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_converted_from_Exception_sets_InnerException()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        Error sut = exception;
        Assert.That(sut.InnerException, Is.EqualTo(exception));
    }

    [Test]
    public void Error_converted_from_Exception_has_no_ValidationResult()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        Error sut = exception;
        Assert.That(sut.ValidationResult, Is.Null);
    }

    [Test]
    public void Error_converted_from_ValidationResult_sets_Message()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        var message = validationResult.ToString();
        Error sut = validationResult;
        Assert.That(sut.Message, Is.EqualTo(message));
    }

    [Test]
    public void Error_converted_from_ValidationResult_has_no_InnerException()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        Error sut = validationResult;
        Assert.That(sut.InnerException, Is.Null);
    }

    [Test]
    public void Error_converted_from_ValidationResult_sets_ValidationResult()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        Error sut = validationResult;
        Assert.That(sut.ValidationResult, Is.EqualTo(validationResult));
    }

    [Test]
    public void Error_ToString_returns_Message()
    {
        var message = _fixture.Create<string>();
        var sut = new Error(message);
        Assert.That(sut.ToString(), Is.EqualTo(message));
    }

    [Test]
    public void Error_AsHttpResult_returns_HttpValidationProblemDetails_for_ValidationResult()
    {
        var property = _fixture.Create<string>();
        var failure = _fixture.Create<string>();
        var validationResult = new ValidationResult([new ValidationFailure(property, failure)]);
        var message = validationResult.ToString();
        var sut = new Error(validationResult);
        var result = sut.AsHttpResult() as ProblemHttpResult;
        Assert.That(result?.ProblemDetails, Is.TypeOf<HttpValidationProblemDetails>());
    }

    [Test]
    public void Error_AsHttpResult_returns_ProblemDetails_for_InnerException()
    {
        var message = _fixture.Create<string>();
        var exception = new InvalidOperationException(message);
        var sut = new Error(exception);
        var result = sut.AsHttpResult() as ProblemHttpResult;
        Assert.That(result?.ProblemDetails, Is.TypeOf<ProblemDetails>());
    }
}
