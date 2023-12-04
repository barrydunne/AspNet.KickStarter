using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter.Tests.AddIns;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInAdditionalConfigurationTests
{
    [Test]
    public void AddInAdditionalConfiguration_configures_builder()
    {
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        WebApplicationBuilder? configured = null;
        var sut = new AddInAdditionalConfiguration { AdditionalConfiguration = (_) => configured = _ };
        sut.Configure(builder);
        Assert.That(configured, Is.EqualTo(builder));
    }

    [Test]
    public void WithAdditionalConfiguration_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithAdditionalConfiguration(AdditionalConfiguration);
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInAdditionalConfiguration>()?.AdditionalConfiguration, Is.EqualTo(AdditionalConfiguration));
#pragma warning restore CS8974 // Converting method group to non-delegate type
        static void AdditionalConfiguration(WebApplicationBuilder builder) => throw new NotImplementedException();
    }
}
