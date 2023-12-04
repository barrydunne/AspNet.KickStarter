using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace AspNet.KickStarter.Demo.Queries;

public record GetDoubleQuery(double Value) : IQuery<double>;
