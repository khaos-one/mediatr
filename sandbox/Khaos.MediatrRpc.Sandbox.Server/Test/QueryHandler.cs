using Khaos.MediatrRpc.Sandbox.Contracts.Test;
using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Server.Test;

public sealed class QueryHandler : IRequestHandler<Query, Result>
{
    public Task<Result> Handle(Query request, CancellationToken cancellationToken) =>
        Task.FromResult(new Result(request.Value + " to you too!"));
}