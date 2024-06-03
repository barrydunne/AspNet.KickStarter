using NSubstitute;
using NUnit.Framework;
using System.Diagnostics.Metrics;

namespace AspNet.KickStarter.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "MeterFactoryExtensions")]
internal class MeterFactoryExtensionsTests
{
    [Test]
    public void CreateAssemblyMeter_should_create_meter_with_assembly_name()
    {
        MeterOptions? created = null;
        var mockMeterFactory = Substitute.For<IMeterFactory>();
        mockMeterFactory
            .Create(Arg.Any<MeterOptions>())
            .Returns(_ => new Meter("test"))
            .AndDoes(callInfo => created = callInfo.ArgAt<MeterOptions>(0));

        var meter = mockMeterFactory.CreateAssemblyMeter();
        
        Assert.That(meter?.Name, Is.EqualTo("test"));
    }
}
