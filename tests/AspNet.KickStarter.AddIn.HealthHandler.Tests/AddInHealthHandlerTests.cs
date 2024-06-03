using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.IO.Abstractions;

namespace AspNet.KickStarter.AddIn.HealthHandler.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInHealthHandlerTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void AddInHealthHandler_adds_services()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetServices(serviceCollection);
        var sut = _fixture.Create<AddInHealthHandler>();

        sut.Configure(builder, []);

        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ImplementationType == typeof(AspNet.KickStarter.HttpHandlers.HealthHandler) && _.Lifetime == ServiceLifetime.Transient), "HealthHandler not registered.");
        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ImplementationType == typeof(FileSystem) && _.Lifetime == ServiceLifetime.Singleton), "FileSystem not registered.");
    }

    [Test]
    public void AddInHealthHandler_adds_endpoints()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        var sut = new AddInHealthHandler
        {
            StatusEndpoint = _fixture.Create<string>(),
            StatusName = _fixture.Create<string>(),
            StatusDescription = _fixture.Create<string>(),
            VersionEndpoint = _fixture.Create<string>(),
            VersionName = _fixture.Create<string>(),
            VersionDescription = _fixture.Create<string>()
        };

        sut.Configure(app, []);
        
        var datasources = PrivateAccessors.GetDataSources(app).FirstOrDefault(_ => typeof(EndpointDataSource).IsAssignableFrom(_.GetType()));
        var endpoints = PrivateAccessors.GetRouteEntries(datasources!);
        Assert.That(endpoints.ConvertAll(_ => _.RoutePattern?.RawText), Is.EquivalentTo(new[] { sut.StatusEndpoint, sut.VersionEndpoint }));
    }

    [Test]
    public void WithHealthHandler_registers_add_in_with_default_properties()
    {
        var statusEndpoint = "/health/status";
        var statusName = "GetHealthStatus";
        var statusDescription = "Check API health";
        var versionEndpoint = "/health/version";
        var versionName = "GetVersion";
        var versionDescription = "Get the API version";
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithHealthHandler();
        
        var addin = apiBuilder.GetAddIn<AddInHealthHandler>();
        Assert.That(addin?.StatusEndpoint, Is.EqualTo(statusEndpoint), "Incorrect StatusEndpoint");
        Assert.That(addin?.StatusName, Is.EqualTo(statusName), "Incorrect StatusName");
        Assert.That(addin?.StatusDescription, Is.EqualTo(statusDescription), "Incorrect StatusDescription");
        Assert.That(addin?.VersionEndpoint, Is.EqualTo(versionEndpoint), "Incorrect VersionEndpoint");
        Assert.That(addin?.VersionName, Is.EqualTo(versionName), "Incorrect VersionName");
        Assert.That(addin?.VersionDescription, Is.EqualTo(versionDescription), "Incorrect VersionDescription");
    }

    [Test]
    public void WithHealthHandler_registers_add_in_with_custom_properties()
    {
        var statusEndpoint = _fixture.Create<string>();
        var statusName = _fixture.Create<string>();
        var statusDescription = _fixture.Create<string>();
        var versionEndpoint = _fixture.Create<string>();
        var versionName = _fixture.Create<string>();
        var versionDescription = _fixture.Create<string>();
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithHealthHandler(statusEndpoint, statusName, statusDescription, versionEndpoint, versionName, versionDescription);
        
        var addin = apiBuilder.GetAddIn<AddInHealthHandler>();
        Assert.That(addin?.StatusEndpoint, Is.EqualTo(statusEndpoint), "Incorrect StatusEndpoint");
        Assert.That(addin?.StatusName, Is.EqualTo(statusName), "Incorrect StatusName");
        Assert.That(addin?.StatusDescription, Is.EqualTo(statusDescription), "Incorrect StatusDescription");
        Assert.That(addin?.VersionEndpoint, Is.EqualTo(versionEndpoint), "Incorrect VersionEndpoint");
        Assert.That(addin?.VersionName, Is.EqualTo(versionName), "Incorrect VersionName");
        Assert.That(addin?.VersionDescription, Is.EqualTo(versionDescription), "Incorrect VersionDescription");
    }
}
