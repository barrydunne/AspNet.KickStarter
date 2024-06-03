using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.KickStarter.CQRS;

/// <summary>
/// Extension methods for registering pipeline behaviors.
/// </summary>
public static class PipelineExtensions
{
    /// <summary>
    /// Registers the TracePipelineBehavior with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddTracePipelineBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(TracePipelineBehavior<,>));

    /// <summary>
    /// Registers the ValidationPipelineBehavior with the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddValidationPipelineBehavior(this IServiceCollection services)
        => services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
}
