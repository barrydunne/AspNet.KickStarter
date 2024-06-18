using AspNet.KickStarter.FunctionalResult;
using MediatR;

namespace AspNet.KickStarter.CQRS.Abstractions.Commands;

/// <summary>
/// Indicates a CQRS command.
/// </summary>
public interface ICommand : IRequest<Result> { }

/// <summary>
/// Indicates a CQRS command with a typed response to allow returning the ID of a new entity.
/// </summary>
/// <typeparam name="TResult">The type of result from the command.</typeparam>
public interface ICommand<TResult> : IRequest<Result<TResult>> { }
