using AspNet.KickStarter.FunctionalResult.Extensions;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests.Extensions;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "MatchExtensions")]
internal class MatchExtensionsTests
{
    [Test]
    public void Result_Match_invokes_success()
    {
        var sut = Result.Success();
        var match = sut.Match(() => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
    }

    [Test]
    public void Result_Match_invokes_error()
    {
        Result sut = new Error();
        var match = sut.Match(() => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
    }

    [Test]
    public async Task Result_MatchAsync_invokes_success()
    {
        var sut = Result.Success();
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
        Task<SuccessOrError> OnSuccess() => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    [Test]
    public async Task Result_MatchAsync_invokes_error()
    {
        Result sut = new Error();
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
        Task<SuccessOrError> OnSuccess() => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    [Test]
    public void ResultT_Match_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var match = sut.Match(value => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
    }

    [Test]
    public void ResultT_Match_invokes_error()
    {
        Result<int> sut = new Error();
        var match = sut.Match(value => SuccessOrError.Success, error => SuccessOrError.Error);
        Assert.That(match, Is.EqualTo(SuccessOrError.Error));
    }

    [Test]
    public async Task ResultT_MatchAsync_invokes_success()
    {
        var sut = Result<int>.Success(1);
        var match = await sut.MatchAsync(OnSuccess, OnError);
        Assert.That(match, Is.EqualTo(SuccessOrError.Success));
        Task<SuccessOrError> OnSuccess(int value) => Task.FromResult(SuccessOrError.Success);
        Task<SuccessOrError> OnError(Error error) => Task.FromResult(SuccessOrError.Error);
    }

    [Test]
    public async Task ResultT_MatchAsync_invokes_error()
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
