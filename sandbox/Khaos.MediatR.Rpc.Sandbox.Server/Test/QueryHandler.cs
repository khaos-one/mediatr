using Khaos.MediatR.Rpc.Sandbox.Contracts.Test;

using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Server.Test;

public sealed class QueryHandler : IRequestHandler<Query, Result>
{
    public Task<Result> Handle(Query request, CancellationToken cancellationToken) =>
        Task.FromResult(new Result(request.Value + " to you too!"));
}