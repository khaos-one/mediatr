using Khaos.MediatR.Rpc.Sandbox.Contracts.TestWithoutReturnType;

using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Server.TestWithoutReturnType;

public sealed class CommandHandler : IRequestHandler<Command>
{
    public async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync(request.Value);
    }
}