using AspNet.KickStarter.Core.Tests.AddIns;
using Microsoft.AspNetCore.Builder;
using NUnit.Framework;

namespace AspNet.KickStarter.AddIn.Endpoints.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInEndpointsTests
{
    [Test]
    public void AddInEndpoints_configures_app()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        WebApplication? configured = null;
        var sut = new AddInEndpoints { Endpoints = (_) => configured = _ };

        sut.Configure(app, []);
        
        Assert.That(configured, Is.EqualTo(app));
    }

    [Test]
    public void WithEndpoints_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        static void MapEndpoints(WebApplication app) => throw new NotImplementedException();

        apiBuilder.WithEndpoints(MapEndpoints);

#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInEndpoints>()?.Endpoints, Is.EqualTo(MapEndpoints));
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }
}
