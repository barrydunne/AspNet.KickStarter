using MediatR;

namespace AspNet.KickStarter.CQRS.Abstractions.Queries;

/// <summary>
/// Indicates a CQRS query handler.
/// </summary>
/// <typeparam name="TQuery">The type of the query.</typeparam>
/// <typeparam name="TResult">The type of result from the query.</typeparam>
public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, Result<TResult>> where TQuery : IRequest<Result<TResult>> { }
