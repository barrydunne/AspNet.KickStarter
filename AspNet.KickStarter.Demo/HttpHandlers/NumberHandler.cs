using AspNet.KickStarter.Demo.Models;
using AspNet.KickStarter.Demo.Queries;
using Mapster;
using MediatR;

namespace AspNet.KickStarter.Demo.HttpHandlers;

public class NumberHandler
{
    private readonly ISender _mediator;
    private readonly ILogger<NumberHandler> _logger;

    public NumberHandler(ISender mediator, ILogger<NumberHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    internal async Task<IResult> GetDoubleAsync(GetDoubleRequest request)
    {
        var result = await _mediator.Send(request.Adapt<GetDoubleQuery>());
        return result.Match(
            success => Results.Ok(success.Adapt<GetDoubleResponse>()),
            error => error.AsHttpResult());
    }
}
