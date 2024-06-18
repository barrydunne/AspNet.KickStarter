using AspNet.KickStarter.FunctionalResult;
using MediatR;

namespace AspNet.KickStarter.CQRS.Abstractions.Commands;

/// <summary>
/// Indicates a CQRS command handler.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result> where TCommand : IRequest<Result> { }

/// <summary>
/// Indicates a CQRS command handler.
/// </summary>
/// <typeparam name="TCommand">The type of the command.</typeparam>
/// <typeparam name="TResult">The type of result from the command.</typeparam>
public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, Result<TResult>> where TCommand : IRequest<Result<TResult>> { }
