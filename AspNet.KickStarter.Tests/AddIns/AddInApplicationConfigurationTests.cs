using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter.Tests.AddIns;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInApplicationConfigurationTests
{
    [Test]
    public void AddInApplicationConfiguration_configures_builder()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        WebApplication? configured = null;
        var sut = new AddInApplicationConfiguration { ApplicationConfiguration = (_) => configured = _ };
        sut.Configure(app);
        Assert.That(configured, Is.EqualTo(app));
    }

    [Test]
    public void WithApplicationConfiguration_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithApplicationConfiguration(ApplicationConfiguration);
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInApplicationConfiguration>()?.ApplicationConfiguration, Is.EqualTo(ApplicationConfiguration));
#pragma warning restore CS8974 // Converting method group to non-delegate type
        static void ApplicationConfiguration(WebApplication app) => throw new NotImplementedException();
    }
}
