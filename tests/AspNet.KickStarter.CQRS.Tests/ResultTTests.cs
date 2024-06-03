using AutoFixture;
using NUnit.Framework;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "Result")]
internal class ResultTTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void ResultT_success_constructor_is_successful()
    {
        var sut = new Result<int>(1);
        Assert.That(sut.IsSuccess, Is.True);
    }

    [Test]
    public void ResultT_success_constructor_has_Value()
    {
        var value = _fixture.Create<int>();
        var sut = new Result<int>(value);
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    public void ResultT_success_constructor_is_not_error()
    {
        var sut = new Result<int>(1);
        Assert.That(sut.IsError, Is.False);
    }

    [Test]
    public void ResultT_success_constructor_has_no_error()
    {
        var sut = new Result<int>(1);
        Assert.That(sut.Error, Is.Null);
    }

    [Test]
    public void ResultT_Error_constructor_is_not_successful()
    {
        var sut = new Result<string>(new Error());
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void ResultT_Error_constructor_is_error()
    {
        var sut = new Result<string>(new Error());
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void ResultT_Error_constructor_has_no_Value()
    {
        var sut = new Result<string>(new Error());
        Assert.That(sut.Value, Is.Null);
    }

    [Test]
    public void ResultT_Error_constructor_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        var sut = new Result<string>(error);
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void ResultT_converted_from_value_is_successful()
    {
        Result<int> sut = 1;
        Assert.That(sut.IsSuccess, Is.True);
    }

    [Test]
    public void ResultT_converted_from_value_is_not_error()
    {
        Result<int> sut = 1;
        Assert.That(sut.IsError, Is.False);
    }

    [Test]
    public void ResultT_converted_from_value_has_Value()
    {
        var value = _fixture.Create<int>();
        Result<int> sut = value;
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    public void ResultT_converted_from_value_has_no_Error()
    {
        Result<int> sut = 1;
        Assert.That(sut.Error, Is.Null);
    }

    [Test]
    public void ResultT_converted_from_Error_is_not_successful()
    {
        Result<string> sut = new Error();
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void ResultT_converted_from_Error_is_error()
    {
        Result<string> sut = new Error();
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void ResultT_converted_from_Error_has_no_Value()
    {
        Result<string> sut = new Error();
        Assert.That(sut.Value, Is.Null);
    }

    [Test]
    public void ResultT_converted_from_Error_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        Result<string> sut = error;
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void ResultT_converted_from_Exception_is_not_successful()
    {
        Result<string> sut = new InvalidOperationException();
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void ResultT_converted_from_Exception_is_error()
    {
        Result<string> sut = new InvalidOperationException();
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void ResultT_converted_from_Exception_has_no_Value()
    {
        Result<string> sut = new InvalidOperationException();
        Assert.That(sut.Value, Is.Null);
    }

    [Test]
    public void ResultT_converted_from_Exception_has_Error()
    {
        Result<string> sut = new InvalidOperationException();
        Assert.That(sut.Error, Is.Not.Null);
    }

    [Test]
    public void ResultT_FromError_is_not_successful()
    {
        var sut = Result<string>.FromError(new Error());
        Assert.That(sut.IsSuccess, Is.False);
    }

    [Test]
    public void ResultT_FromError_is_error()
    {
        var sut = Result<string>.FromError(new Error());
        Assert.That(sut.IsError, Is.True);
    }

    [Test]
    public void ResultT_FromError_has_no_Value()
    {
        var sut = Result<string>.FromError(new Error());
        Assert.That(sut.Value, Is.Null);
    }

    [Test]
    public void ResultT_FromError_has_Error()
    {
        var error = new Error(_fixture.Create<string>());
        var sut = Result<string>.FromError(error);
        Assert.That(sut.Error, Is.EqualTo(error));
    }

    [Test]
    public void ResultT_Success_is_successful()
    {
        var sut = Result<int>.Success(1);
        Assert.That(sut.IsSuccess, Is.True);
    }

    [Test]
    public void ResultT_Success_is_not_error()
    {
        var sut = Result<int>.Success(1);
        Assert.That(sut.IsError, Is.False);
    }

    [Test]
    public void ResultT_Success_is_has_Value()
    {
        var value = _fixture.Create<int>();
        var sut = Result<int>.Success(value);
        Assert.That(sut.Value, Is.EqualTo(value));
    }

    [Test]
    public void ResultT_Success_has_no_error()
    {
        var sut = Result<int>.Success(1);
        Assert.That(sut.Error, Is.Null);
    }

    [Test]
    public void ResultT_Switch_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var successInvoked = 0;
        var errorInvoked = 0;
        sut.Switch(value => successInvoked++, error => errorInvoked++);
        Assert.That(successInvoked, Is.EqualTo(1));
        Assert.That(errorInvoked, Is.EqualTo(0));
    }

    [Test]
    public void ResultT_Switch_invokes_error()
    {
        Result<int> sut = new Error();
        var successInvoked = 0;
        var errorInvoked = 0;
        sut.Switch(value => successInvoked++, error => errorInvoked++);
        Assert.That(successInvoked, Is.EqualTo(0));
        Assert.That(errorInvoked, Is.EqualTo(1));
    }

    [Test]
    public async Task ResultT_SwitchAsync_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var successInvoked = 0;
        var errorInvoked = 0;
        await sut.SwitchAsync(OnSuccess, OnError);
        Assert.That(successInvoked, Is.EqualTo(1));
        Assert.That(errorInvoked, Is.EqualTo(0));
        Task OnSuccess(int value) => Task.FromResult(++successInvoked);
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public async Task ResultT_SwitchAsync_invokes_error()
    {
        Result<int> sut = new Error();
        var successInvoked = 0;
        var errorInvoked = 0;
        await sut.SwitchAsync(OnSuccess, OnError);
        Assert.That(successInvoked, Is.EqualTo(0));
        Assert.That(errorInvoked, Is.EqualTo(1));
        Task OnSuccess(int value) => Task.FromResult(++successInvoked);
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public void ResultT_IfSuccess_invokes_with_success()
    {
        var sut = Result<int>.Success(1);
        var successInvoked = 0;
        sut.IfSuccess(value => successInvoked++);
        Assert.That(successInvoked, Is.EqualTo(1));
    }

    [Test]
    public void ResultT_IfSuccess_does_not_invoke_with_error()
    {
        Result<int> sut = new Error();
        var successInvoked = 0;
        sut.IfSuccess(value => successInvoked++);
        Assert.That(successInvoked, Is.EqualTo(0));
    }

    [Test]
    public async Task ResultT_IfSuccessAsync_invokes_with_success()
    {
        var sut = Result<int>.Success(1);
        var successInvoked = 0;
        await sut.IfSuccessAsync(OnSuccess);
        Assert.That(successInvoked, Is.EqualTo(1));
        Task OnSuccess(int value) => Task.FromResult(++successInvoked);
    }

    [Test]
    public async Task ResultT_IfSuccessAsync_does_not_invoke_with_error()
    {
        Result<int> sut = new Error();
        var successInvoked = 0;
        await sut.IfSuccessAsync(OnSuccess);
        Assert.That(successInvoked, Is.EqualTo(0));
        Task OnSuccess(int value) => Task.FromResult(++successInvoked);
    }

    [Test]
    public void ResultT_IfError_does_not_invoke_with_success()
    {
        var sut = Result<int>.Success(1);
        var errorInvoked = 0;
        sut.IfError((error) => errorInvoked++);
        Assert.That(errorInvoked, Is.EqualTo(0));
    }

    [Test]
    public void ResultT_IfError_invokes_with_error()
    {
        Result<int> sut = new Error();
        var errorInvoked = 0;
        sut.IfError((error) => errorInvoked++);
        Assert.That(errorInvoked, Is.EqualTo(1));
    }

    [Test]
    public async Task ResultT_IfErrorAsync_does_not_invoke_with_success()
    {
        var sut = Result<int>.Success(1);
        var errorInvoked = 0;
        await sut.IfErrorAsync(OnError);
        Assert.That(errorInvoked, Is.EqualTo(0));
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public async Task ResultT_IfErrorAsync_invokes_with_error()
    {
        Result<int> sut = new Error();
        var errorInvoked = 0;
        await sut.IfErrorAsync(OnError);
        Assert.That(errorInvoked, Is.EqualTo(1));
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public void Match_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var match = sut.Match(value => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
    }

    [Test]
    public void Match_invokes_error()
    {
        Result<int> sut = new Error();
        var match = sut.Match(value => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
    }

    [Test]
    public async Task MatchAsync_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
        Task<SuccessOrError> OnSuccess(int value) => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    [Test]
    public async Task MatchAsync_invokes_error()
    {
        Result<int> sut = new Error();
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
        Task<SuccessOrError> OnSuccess(int value) => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    private enum SuccessOrError
    {
        Success = 1,
        Error = 2
    }
}
