using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Client.LocalCommand;

public sealed class CommandHandler : IRequestHandler<Command>
{
    public Task Handle(Command request, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Local command: {request.Value}");
        
        return Task.CompletedTask;
    }
}