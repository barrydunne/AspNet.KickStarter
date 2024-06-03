using AspNet.KickStarter.Core.Tests.AddIns;
using Microsoft.AspNetCore.Builder;
using NUnit.Framework;

namespace AspNet.KickStarter.AddIn.Services.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInServicesTests
{
    [Test]
    public void AddInServices_configures_builder()
    {
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        WebApplicationBuilder? configured = null;
        var sut = new AddInServices { RegisterServices = (_) => configured = _ };

        sut.Configure(builder, []);

        Assert.That(configured, Is.EqualTo(builder));
    }

    [Test]
    public void WithServices_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        static void RegisterServices(WebApplicationBuilder builder) => throw new NotImplementedException();

        apiBuilder.WithServices(RegisterServices);

#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInServices>()?.RegisterServices, Is.EqualTo(RegisterServices));
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }
}
