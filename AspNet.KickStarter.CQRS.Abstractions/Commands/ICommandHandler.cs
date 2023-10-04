using CSharpFunctionalExtensions;
using MediatR;

namespace AspNet.KickStarter.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Indicates a CQRS command.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command.</typeparam>
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result> where TCommand : IRequest<Result> { }
}
