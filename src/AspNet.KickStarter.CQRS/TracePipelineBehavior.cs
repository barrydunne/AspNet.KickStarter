using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AspNet.KickStarter.CQRS;

/// <summary>
/// Records trace data for the handler.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class TracePipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IResult
{
    private readonly ITraceActivity? _traceActivity;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="TracePipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="traceActivity">The source to start a new <see cref="Activity"/> to record the handle method.</param>
    /// <param name="logger">The logger to write to.</param>
    public TracePipelineBehavior(ITraceActivity traceActivity, ILogger<TracePipelineBehavior<TRequest, TResponse>> logger)
    {
        _traceActivity = traceActivity;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TracePipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    public TracePipelineBehavior(ILogger<TracePipelineBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        using var trace = _traceActivity?.StartActivity($"{typeof(TRequest).Name} handler");
        return await next();
    }
}
