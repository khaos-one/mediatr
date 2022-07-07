using Khaos.MediatrRpc.Sandbox.Contracts.TestWithoutReturnType;

using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Server.TestWithoutReturnType;

public sealed class CommandHandler : AsyncRequestHandler<Command>
{
    protected override async Task Handle(Command request, CancellationToken cancellationToken)
    {
        await Console.Out.WriteLineAsync(request.Value);
    }
}