using AspNet.KickStarter.FunctionalResult;
using AutoFixture;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "TracePipelineBehavior")]
internal class TracePipelineBehaviorTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public async Task TracePipelineBehavior_without_trace_activity_returns_next()
    {
        var mockLogger = Substitute.For<ILogger<TracePipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new TracePipelineBehavior<TestQuery, Result<int>>(mockLogger);

        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);

        Assert.That(result.Value, Is.EqualTo(nextValue));
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }

    [Test]
    public async Task TracePipelineBehavior_with_trace_starts_activity_and_returns_next()
    {
        var mockTraceActivity = Substitute.For<ITraceActivity>();
        var mockLogger = Substitute.For<ILogger<TracePipelineBehavior<TestQuery, Result<int>>>>();
        var nextValue = _fixture.Create<int>();
        var sut = new TracePipelineBehavior<TestQuery, Result<int>>(mockTraceActivity, mockLogger);

        var result = await sut.Handle(new TestQuery(), Next, CancellationToken.None);

        Assert.That(result.Value, Is.EqualTo(nextValue));
        mockTraceActivity.Received(1).StartActivity($"{nameof(TestQuery)} handler");
        Task<Result<int>> Next() => Task.FromResult(new Result<int>(nextValue));
    }
}
