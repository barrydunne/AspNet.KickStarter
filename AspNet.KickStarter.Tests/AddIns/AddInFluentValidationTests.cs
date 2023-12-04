using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.KickStarter.Tests.AddIns;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "AddIns")]
internal class AddInFluentValidationTests
{
    [TestCase(ServiceLifetime.Scoped)]
    [TestCase(ServiceLifetime.Singleton)]
    [TestCase(ServiceLifetime.Transient)]
    public void AddInFluentValidation_adds_validators(ServiceLifetime lifetime)
    {
        var serviceCollection = new ServiceCollection();
        var builder = WebApplication.CreateBuilder(Array.Empty<string>());
        builder.SetServices(serviceCollection);
        var sut = new AddInFluentValidation
        {
            Type = GetType(),
            Lifetime = lifetime,
            IncludeInternalTypes = true
        };
        sut.Configure(builder);
        Assert.That(serviceCollection, Has.Some.Matches<ServiceDescriptor>(_ => (_.ImplementationType == typeof(TestValidator) && (_.Lifetime == lifetime))));
    }

    [Test]
    public void WithFluentValidation_registers_add_in_with_default_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithFluentValidationFromAssemblyContaining<AddInFluentValidationTests>();
        var addin = apiBuilder.GetAddIn<AddInFluentValidation>();
        Assert.That(addin?.Type, Is.EqualTo(typeof(AddInFluentValidationTests)), "Incorrect Type");
        Assert.That(addin?.Lifetime, Is.EqualTo(ServiceLifetime.Scoped), "Incorrect Lifetime");
        Assert.That(addin?.Filter, Is.Null, "Incorrect Filter");
        Assert.That(addin?.IncludeInternalTypes, Is.True, "Incorrect IncludeInternalTypes");
    }

    [Test]
    public void WithFluentValidation_registers_add_in_with_custom_properties()
    {
        var apiBuilder = new ApiBuilder();
        apiBuilder.WithFluentValidationFromAssemblyContaining<AddInFluentValidationTests>(ServiceLifetime.Singleton, Filter, false);
        var addin = apiBuilder.GetAddIn<AddInFluentValidation>();
        Assert.That(addin?.Type, Is.EqualTo(typeof(AddInFluentValidationTests)), "Incorrect Type");
        Assert.That(addin?.Lifetime, Is.EqualTo(ServiceLifetime.Singleton), "Incorrect Lifetime");
#pragma warning disable CS8974 // Converting method group to non-delegate type
        Assert.That(addin?.Filter, Is.EqualTo(Filter), "Incorrect Filter");
#pragma warning restore CS8974 // Converting method group to non-delegate type
        Assert.That(addin?.IncludeInternalTypes, Is.False, "Incorrect IncludeInternalTypes");
        static bool Filter(AssemblyScanner.AssemblyScanResult result) => throw new NotImplementedException();
    }

    internal class TestValidator : AbstractValidator<AddInFluentValidation> { }
}
