using AutoFixture;
using NUnit.Framework;
using System.Diagnostics;

namespace AspNet.KickStarter.Tracing.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "TraceActivity")]
internal class TraceActivityTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void Name_should_report_name()
    {
        var sourceName = _fixture.Create<string>();

        using var sut = new TraceActivity(sourceName);

        Assert.That(sut.Name, Is.EqualTo(sourceName));
    }

    [Test]
    public void StartActivity_should_create_activity_with_name()
    {
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = _ => true;
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData;
        listener.ActivityStopped += _ => activities.Add(_);
        ActivitySource.AddActivityListener(listener);
        var sourceName = _fixture.Create<string>();
        var activityName = _fixture.Create<string>();
        using var sut = new TraceActivity(sourceName);

        using var activity = sut.StartActivity(activityName);

        Assert.That(activity!.Source.Name, Is.EqualTo(sourceName));
        Assert.That(activity.OperationName, Is.EqualTo(activityName));
    }

    [Test]
    public void StartActivity_should_create_activity_without_name()
    {
        var activities = new List<Activity>();
        using var listener = new ActivityListener();
        listener.ShouldListenTo = _ => true;
        listener.Sample = (ref ActivityCreationOptions<ActivityContext> options) => ActivitySamplingResult.AllData;
        listener.ActivityStopped += _ => activities.Add(_);
        ActivitySource.AddActivityListener(listener);
        var sourceName = _fixture.Create<string>();
        var activityName = _fixture.Create<string>();
        using var sut = new TraceActivity(sourceName);

        using var activity = sut.StartActivity();

        Assert.That(activity!.Source.Name, Is.EqualTo(sourceName));
        Assert.That(activity.OperationName, Is.EqualTo(nameof(StartActivity_should_create_activity_without_name)));
    }
}
