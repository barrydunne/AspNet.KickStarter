using AutoFixture;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "Result")]
internal class ResultTests
{
    private readonly Fixture _fixture;

    public ResultTests() => _fixture = new();

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

    [Test]
    public void Result_Switch_invokes_success()
    {
        var sut = Result.Success();
        var successInvoked = 0;
        var errorInvoked = 0;
        sut.Switch(() => successInvoked++, error => errorInvoked++);
        Assert.That(successInvoked, Is.EqualTo(1));
        Assert.That(errorInvoked, Is.EqualTo(0));
    }

    [Test]
    public void Result_Switch_invokes_error()
    {
        Result sut = new Error();
        var successInvoked = 0;
        var errorInvoked = 0;
        sut.Switch(() => successInvoked++, error => errorInvoked++);
        Assert.That(successInvoked, Is.EqualTo(0));
        Assert.That(errorInvoked, Is.EqualTo(1));
    }

    [Test]
    public async Task Result_SwitchAsync_invokes_success()
    {
        var sut = Result.Success();
        var successInvoked = 0;
        var errorInvoked = 0;
        await sut.SwitchAsync(OnSuccess, OnError);
        Assert.That(successInvoked, Is.EqualTo(1));
        Assert.That(errorInvoked, Is.EqualTo(0));
        Task OnSuccess() => Task.FromResult(++successInvoked);
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public async Task Result_SwitchAsync_invokes_error()
    {
        Result sut = new Error();
        var successInvoked = 0;
        var errorInvoked = 0;
        await sut.SwitchAsync(OnSuccess, OnError);
        Assert.That(successInvoked, Is.EqualTo(0));
        Assert.That(errorInvoked, Is.EqualTo(1));
        Task OnSuccess() => Task.FromResult(++successInvoked);
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public void Result_IfSuccess_invokes_with_success()
    {
        var sut = Result.Success();
        var successInvoked = 0;
        sut.IfSuccess(() => successInvoked++);
        Assert.That(successInvoked, Is.EqualTo(1));
    }

    [Test]
    public void Result_IfSuccess_does_not_invoke_with_error()
    {
        Result sut = new Error();
        var successInvoked = 0;
        sut.IfSuccess(() => successInvoked++);
        Assert.That(successInvoked, Is.EqualTo(0));
    }

    [Test]
    public async Task Result_IfSuccessAsync_invokes_with_success()
    {
        var sut = Result.Success();
        var successInvoked = 0;
        await sut.IfSuccessAsync(OnSuccess);
        Assert.That(successInvoked, Is.EqualTo(1));
        Task OnSuccess() => Task.FromResult(++successInvoked);
    }

    [Test]
    public async Task Result_IfSuccessAsync_does_not_invoke_with_error()
    {
        Result sut = new Error();
        var successInvoked = 0;
        await sut.IfSuccessAsync(OnSuccess);
        Assert.That(successInvoked, Is.EqualTo(0));
        Task OnSuccess() => Task.FromResult(++successInvoked);
    }

    [Test]
    public void Result_IfError_does_not_invoke_with_success()
    {
        var sut = Result.Success();
        var errorInvoked = 0;
        sut.IfError((error) => errorInvoked++);
        Assert.That(errorInvoked, Is.EqualTo(0));
    }

    [Test]
    public void Result_IfError_invokes_with_error()
    {
        Result sut = new Error();
        var errorInvoked = 0;
        sut.IfError((error) => errorInvoked++);
        Assert.That(errorInvoked, Is.EqualTo(1));
    }

    [Test]
    public async Task Result_IfErrorAsync_does_not_invoke_with_success()
    {
        var sut = Result.Success();
        var errorInvoked = 0;
        await sut.IfErrorAsync(OnError);
        Assert.That(errorInvoked, Is.EqualTo(0));
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public async Task Result_IfErrorAsync_invokes_with_error()
    {
        Result sut = new Error();
        var errorInvoked = 0;
        await sut.IfErrorAsync(OnError);
        Assert.That(errorInvoked, Is.EqualTo(1));
        Task OnError(Error error) => Task.FromResult(++errorInvoked);
    }

    [Test]
    public void Match_invokes_success()
    {
        var sut = Result.Success();
        var match = sut.Match(() => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
    }

    [Test]
    public void Match_invokes_error()
    {
        Result sut = new Error();
        var match = sut.Match(() => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
    }

    [Test]
    public async Task MatchAsync_invokes_success()
    {
        var sut = Result.Success();
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
        Task<SuccessOrError> OnSuccess() => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    [Test]
    public async Task MatchAsync_invokes_error()
    {
        Result sut = new Error();
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
        Task<SuccessOrError> OnSuccess() => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    private enum SuccessOrError
    {
        Success = 1,
        Error = 2
    }
}
