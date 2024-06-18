using AspNet.KickStarter.FunctionalResult.Extensions;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests.Extensions;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "IfErrorExtensions")]
internal class IfErrorExtensionsTests
{
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
}
