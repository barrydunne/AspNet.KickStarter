using AspNet.KickStarter.FunctionalResult.Extensions;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests.Extensions;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "SwitchExtensions")]
internal class SwitchExtensionsTests
{
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
}
