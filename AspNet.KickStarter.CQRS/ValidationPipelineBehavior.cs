using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace AspNet.KickStarter.CQRS;

/// <summary>
/// Performs validation during the pipeline of command/query handing.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type.</typeparam>
public class ValidationPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse> where TResponse : IResult
{
    private readonly IValidator<TRequest>? _validator;
    private readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validator">The request validator.</param>
    /// <param name="logger">The logger to write to.</param>
    public ValidationPipelineBehavior(IValidator<TRequest> validator, ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationPipelineBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">The logger to write to.</param>
    public ValidationPipelineBehavior(ILogger<ValidationPipelineBehavior<TRequest, TResponse>> logger) => _logger = logger;

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Input validation, if available
        if (_validator is not null)
        {
            _logger.LogDebug("Performing {Type} validation.", typeof(TRequest).Name);

            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                return (dynamic)(Error)validationResult;
        }

        return await next();
    }
}
