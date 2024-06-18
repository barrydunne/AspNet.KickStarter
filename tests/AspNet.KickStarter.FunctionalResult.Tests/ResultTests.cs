using AutoFixture;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "Result")]
internal class ResultTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Result_default_constructor_is_successful()
    {
        var sut = new Result();
        Assert.That(sut.IsSuccess, Is.True);
    }

    [Test]
    public void Result_default_constructor_is_not_error()
    {
        var sut = new Result();
        Assert.That(sut.IsError, Is.False);
    }

    [Test]
    public void Result_default_constructor_has_no_error()
    {
        var sut = new Result();
        Assert.That(sut.Error, Is.Null);
    }

    [Test]
    public void Result_Error_constructor_is_not_successful()
    {
        var sut = new Result(new Error());
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void Result_Error_constructor_is_error()
    {
        var sut = new Result(new Error());
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void Result_Error_constructor_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        var sut = new Result(error);
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void Result_converted_from_Error_is_not_successful()
    {
        Result sut = new Error();
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void Result_converted_from_Error_is_error()
    {
        Result sut = new Error();
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void Result_converted_from_Error_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        Result sut = error;
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void Result_converted_from_Exception_is_not_successful()
    {
        Result sut = new InvalidOperationException();
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void Result_converted_from_Exception_is_error()
    {
        Result sut = new InvalidOperationException();
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void Result_converted_from_Exception_has_Error()
    {
        Result sut = new InvalidOperationException();
        Assert.That(sut.Error, Is.Not.Null);
    }

    [Test]
    public void Result_FromError_is_not_successful()
    {
        var sut = Result.FromError(new Error());
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void Result_FromError_is_error()
    {
        var sut = Result.FromError(new Error());
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void Result_FromError_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        var sut = Result.FromError(error);
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void Result_Success_is_successful()
    {
        var sut = Result.Success();
        Assert.That(sut.IsSuccess, Is.True);
    }

    [Test]
    public void Result_Success_is_not_error()
    {
        var sut = Result.Success();
        Assert.That(sut.IsError, Is.False);
    }

    [Test]
    public void Result_Success_has_no_error()
    {
        var sut = Result.Success();
        Assert.That(sut.Error, Is.Null);
    }
}
