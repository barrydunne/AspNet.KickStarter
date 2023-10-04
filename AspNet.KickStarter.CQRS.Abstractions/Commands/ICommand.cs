using CSharpFunctionalExtensions;
using MediatR;

namespace AspNet.KickStarter.CQRS.Abstractions.Commands
{
    /// <summary>
    /// Indicates a CQRS command.
    /// </summary>
    public interface ICommand : IRequest<Result> { }
}
