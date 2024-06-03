using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AspNet.KickStarter.AddIn.Swagger.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInSwaggerTests
{
    private readonly Fixture _fixture = new();

    [Test]
    public void AddInSwagger_adds_services()
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetServices(serviceCollection);
        var sut = new AddInSwagger { WithSwaggerBearerToken = true, WithSwaggerOnlyInDevelopment = false };

        sut.Configure(builder, []);

        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ImplementationType == typeof(SwaggerGenerator)));
    }

    [Test]
    public void AddInSwagger_adds_middleware()
    {
        var app = WebApplication.CreateBuilder(Array.Empty<string>()).Build();
        var sut = new AddInSwagger { WithSwaggerBearerToken = true, WithSwaggerOnlyInDevelopment = false };

        sut.Configure(app, []);

        var applicationBuilder = PrivateAccessors.GetApplicationBuilder(app);
        var components = PrivateAccessors.GetComponents(applicationBuilder);
        Assert.That(components, Is.Not.Empty);
    }

    [Test]
    public void WithSwagger_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithSwagger();

        var addin = apiBuilder.GetAddIn<AddInSwagger>();
        Assert.That(addin?.WithSwaggerBearerToken, Is.False, "Incorrect WithSwaggerBearerToken");
        Assert.That(addin?.WithSwaggerOnlyInDevelopment, Is.False, "Incorrect WithSwaggerOnlyInDevelopment");
    }

    [Test]
    public void WithSwagger_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();

        apiBuilder.WithSwagger(true, true);

        var addin = apiBuilder.GetAddIn<AddInSwagger>();
        Assert.That(addin?.WithSwaggerBearerToken, Is.True, "Incorrect WithSwaggerBearerToken");
        Assert.That(addin?.WithSwaggerOnlyInDevelopment, Is.True, "Incorrect WithSwaggerOnlyInDevelopment");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void AddInSwaggger_SetupSwaggerGenOptions_adds_bearer_if_required(bool withSwaggerBearerToken)
    {
        var options = new SwaggerGenOptions();
        var sut = new AddInSwagger { WithSwaggerBearerToken = withSwaggerBearerToken };

        sut.SetupSwaggerGenOptions(options);

        Assert.That(options.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer"), Is.EqualTo(withSwaggerBearerToken));
    }
}
