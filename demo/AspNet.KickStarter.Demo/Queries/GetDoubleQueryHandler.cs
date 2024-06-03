using AspNet.KickStarter.CQRS;
using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace AspNet.KickStarter.Demo.Queries;

internal class GetDoubleQueryHandler : IQueryHandler<GetDoubleQuery, double>
{
    private readonly ILogger _logger;

    public GetDoubleQueryHandler(ILogger<GetDoubleQueryHandler> logger) => _logger = logger;

    public Task<Result<double>> Handle(GetDoubleQuery request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("GetDoubleQueryHandler for value: {Value}", request.Value);

        Result<double> result;
        try
        {
            result = 2 * request.Value;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to double number. Request: {Request}", request);
            result = ex;
        }
        return Task.FromResult(result);
    }
}
