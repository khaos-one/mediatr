using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Client.LocalCommand;

public sealed class CommandHandler : AsyncRequestHandler<Command>
{
    protected override Task Handle(Command request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Local command: {request.Value}");
        
        return Task.CompletedTask;
    }
}