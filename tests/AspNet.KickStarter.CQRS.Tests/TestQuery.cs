using AspNet.KickStarter.CQRS.Abstractions.Queries;

namespace AspNet.KickStarter.CQRS.Tests;

public record TestQuery : IQuery<int>;
