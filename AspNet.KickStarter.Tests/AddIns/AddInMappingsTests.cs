using Microsoft.AspNetCore.Builder;

namespace AspNet.KickStarter.Tests.AddIns;

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
        sut.Configure(app);
        Assert.That(configured, Is.True);
    }

    [Test]
    public void WithMappings_registers_add_in()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithMappings(Mapper);
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInMappings>()?.Mapper, Is.EqualTo(Mapper));
#pragma warning restore CS8974 // Converting method group to non-delegate type
        static void Mapper() => throw new NotImplementedException();
    }
}
