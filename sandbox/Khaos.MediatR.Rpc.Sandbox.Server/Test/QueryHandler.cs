using Khaos.MediatR.Callable;
using Khaos.MediatR.Rpc.Sandbox.Contracts.Test;
using Khaos.MediatR.Rpc.Sandbox.Contracts.TestWithoutReturnType;

using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Server.Test;

public sealed class QueryHandler : IRequestHandler<Query, Result>
{
    private readonly ICall<Contracts.TestWithoutReturnType.Command> _testWithoutReturnType;

    public QueryHandler(ICall<Command> testWithoutReturnType)
    {
        _testWithoutReturnType = testWithoutReturnType;
    }

    public async Task<Result> Handle(Query request, CancellationToken cancellationToken)
    {
        await _testWithoutReturnType.Send(new Command(request.Value), cancellationToken);

        return new Result(request.Value + " to you too!");
    }
}