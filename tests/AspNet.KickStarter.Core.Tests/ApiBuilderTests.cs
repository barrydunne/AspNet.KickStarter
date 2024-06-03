using Microsoft.AspNetCore.Builder;
using NSubstitute;
using NUnit.Framework;

namespace AspNet.KickStarter.Core.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "ApiBuilder")]
internal class ApiBuilderTests
{
    [Test]
    public void ApiBuilder_configures_builder_with_all_addins()
    {
        var mocks = Enumerable.Range(0, 10).Select(_ => CreateMockAddIn()).ToArray();
        var sut = new ApiBuilder();
        foreach (var mock in mocks)
            sut.RegisterAddIn(mock);

        sut.Build([]);

        foreach (var mock in mocks)
            mock.Received(1).Configure(Arg.Any<WebApplicationBuilder>(), Arg.Any<IReadOnlyCollection<IAddIn>>());
    }

    [Test]
    public void ApiBuilder_configures_app_with_all_addins()
    {
        var mocks = Enumerable.Range(0, 10).Select(_ => CreateMockAddIn()).ToArray();
        var sut = new ApiBuilder();
        foreach (var mock in mocks)
            sut.RegisterAddIn(mock);

        sut.Build([]);

        foreach (var mock in mocks)
            mock.Received(1).Configure(Arg.Any<WebApplication>(), Arg.Any<IReadOnlyCollection<IAddIn>>());
    }

    [Test]
    public void AddIns_should_be_empty_when_no_addins_registered()
    {
        var sut = new ApiBuilder();

        sut.Build([]);

        Assert.That(sut.AddIns, Is.Empty);
    }

    [Test]
    public void AddIns_should_contain_all_registered_addins()
    {
        var mocks = Enumerable.Range(0, 10).Select(_ => CreateMockAddIn()).ToArray();
        var sut = new ApiBuilder();
        foreach (var mock in mocks)
            sut.RegisterAddIn(mock);

        sut.Build([]);

        Assert.That(sut.AddIns, Is.EquivalentTo(mocks));
    }

    private IAddIn CreateMockAddIn() => Substitute.For<IAddIn>();
}
