using AspNet.KickStarter.FunctionalResult.Extensions;
using NUnit.Framework;

namespace AspNet.KickStarter.FunctionalResult.Tests.Extensions;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "IfSuccessExtensions")]
internal class IfSuccessExtensionsTests
{
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
}
