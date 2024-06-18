using AspNet.KickStarter.FunctionalResult;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace AspNet.KickStarter.CQRS.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
[TestFixture(Category = "PipelineExtensions")]
public class PipelineExtensionsTests
{
    [Test]
    public void AddTracePipelineBehavior_registers_TracePipelineBehavior()
    {
        var services = new ServiceCollection();
        services
            .AddSingleton(Substitute.For<ITraceActivity>())
            .AddSingleton(Substitute.For<ILogger<TracePipelineBehavior<TestQuery, Result<int>>>>())
            .AddTracePipelineBehavior();

        var provider = services.BuildServiceProvider();
        var behavior = provider.GetRequiredService<IPipelineBehavior<TestQuery, Result<int>>>();
        Assert.That(behavior, Is.InstanceOf<TracePipelineBehavior<TestQuery, Result<int>>>());
    }

    [Test]
    public void AddValidationPipelineBehavior_registers_ValidationPipelineBehavior()
    {
        var services = new ServiceCollection();
        services
            .AddSingleton(Substitute.For<ITraceActivity>())
            .AddSingleton(Substitute.For<ILogger<ValidationPipelineBehavior<TestQuery, Result<int>>>>())
            .AddValidationPipelineBehavior();

        var provider = services.BuildServiceProvider();
        var behavior = provider.GetRequiredService<IPipelineBehavior<TestQuery, Result<int>>>();
        Assert.That(behavior, Is.InstanceOf<ValidationPipelineBehavior<TestQuery, Result<int>>>());
    }
}
