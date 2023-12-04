using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.KickStarter.Tests.AddIns;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInSerilogTests
{
    private readonly Fixture _fixture;

    public AddInSerilogTests() => _fixture = new Fixture();

    [Test]
    public void AddInSerilog_adds_services()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetHostServices(serviceCollection);
        var sut = new AddInSerilog { DebugOutput = (_) => { } };
        sut.Configure(builder);
        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ServiceType == typeof(Serilog.ILogger)));
    }

    [Test]
    public void WithSerilog_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithSerilog();
        var addin = apiBuilder.GetAddIn<AddInSerilog>();
        Assert.That(addin?.DebugOutput, Is.Null);
    }

    [Test]
    public void WithSerilog_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithSerilog(DebugOutput);
        var addin = apiBuilder.GetAddIn<AddInSerilog>();
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(apiBuilder.GetAddIn<AddInSerilog>()?.DebugOutput, Is.EqualTo(DebugOutput));
#pragma warning restore CS8974 // Converting method group to non-delegate type
        static void DebugOutput(string s) => throw new NotImplementedException();
    }
}
