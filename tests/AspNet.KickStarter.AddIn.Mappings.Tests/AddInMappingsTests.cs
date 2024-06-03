using AspNet.KickStarter.Core.Tests.AddIns;
using Microsoft.AspNetCore.Builder;
using NUnit.Framework;

namespace AspNet.KickStarter.AddIn.Mappings.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInMappingsTests
{
    [Test]
    public void AddInMappings_configures_mappings()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        bool? configured = null;
        var sut = new AddInMappings { Mapper = () => configured = true };

        sut.Configure(app, []);
        
        Assert.That(configured, Is.True);
    }

    [Test]
    public void WithMappings_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        static void Mapper() => throw new NotImplementedException();

        apiBuilder.WithMappings(Mapper);

#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInMappings>()?.Mapper, Is.EqualTo(Mapper));
#pragma warning restore CS8974 // Converting method group to non-delegate type
    }
}
