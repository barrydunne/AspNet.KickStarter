using AspNet.KickStarter.Core.Tests.AddIns;
using AutoFixture;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NSwag.Generation.AspNetCore;
using NSwag.Generation.Processors.Security;
using NUnit.Framework;

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

        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => _.ImplementationType?.FullName == "NSwag.AspNetCore.OpenApiDocumentProvider"));
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
        Assert.That(addin?.Title, Is.EqualTo("API Documentation"));
        Assert.That(addin?.Path, Is.EqualTo(""));
        Assert.That(addin?.ReDocPath, Is.EqualTo("/docs"));
        Assert.That(addin?.WithSwaggerBearerToken, Is.False, "Incorrect WithSwaggerBearerToken");
        Assert.That(addin?.WithSwaggerOnlyInDevelopment, Is.False, "Incorrect WithSwaggerOnlyInDevelopment");
    }

    [Test]
    public void WithSwagger_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();
        var title = _fixture.Create<string>();
        var path = _fixture.Create<string>();
        var redoc = _fixture.Create<string>();

        apiBuilder.WithSwagger(title, path, redoc, true, true);

        var addin = apiBuilder.GetAddIn<AddInSwagger>();
        Assert.That(addin?.Title, Is.EqualTo(title));
        Assert.That(addin?.Path, Is.EqualTo(path));
        Assert.That(addin?.ReDocPath, Is.EqualTo(redoc));
        Assert.That(addin?.WithSwaggerBearerToken, Is.True, "Incorrect WithSwaggerBearerToken");
        Assert.That(addin?.WithSwaggerOnlyInDevelopment, Is.True, "Incorrect WithSwaggerOnlyInDevelopment");
    }

    [TestCase(true)]
    [TestCase(false)]
    public void AddInSwaggger_SetupSwaggerGenOptions_adds_bearer_if_required(bool withSwaggerBearerToken)
    {
        var options = new AspNetCoreOpenApiDocumentGeneratorSettings();
        var sut = new AddInSwagger { WithSwaggerBearerToken = withSwaggerBearerToken };

        sut.SetupSwaggerGenOptions(options);

        var def = options.DocumentProcessors.FirstOrDefault(_ => _ is SecurityDefinitionAppender) as SecurityDefinitionAppender;
        if (withSwaggerBearerToken)
        {
            Assert.That(def, Is.Not.Null);
            var nameField = typeof(SecurityDefinitionAppender).GetField("_name", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            var name = nameField?.GetValue(def);
            Assert.That(name, Is.EqualTo("Bearer"));
        }
        else
            Assert.That(def, Is.Null);
    }
}
