using Microsoft.AspNetCore.Builder;
using Moq;

namespace AspNet.KickStarter.Tests;

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
            sut.RegisterAddIn(mock.Object);
        sut.Build([]);
        foreach (var mock in mocks)
            mock.Verify(_ => _.Configure(It.IsAny<WebApplicationBuilder>()));
    }

    [Test]
    public void ApiBuilder_configures_app_with_all_addins()
    {
        var mocks = Enumerable.Range(0, 10).Select(_ => CreateMockAddIn()).ToArray();
        var sut = new ApiBuilder();
        foreach (var mock in mocks)
            sut.RegisterAddIn(mock.Object);
        sut.Build([]);
        foreach (var mock in mocks)
            mock.Verify(_ => _.Configure(It.IsAny<WebApplication>()));
    }

    private Mock<IAddIn> CreateMockAddIn()
    {
        var mock = new Mock<IAddIn>();
        mock.Setup(_ => _.Configure(It.IsAny<WebApplicationBuilder>())).Verifiable();
        mock.Setup(_ => _.Configure(It.IsAny<WebApplication>())).Verifiable();
        return mock;
    }
}
