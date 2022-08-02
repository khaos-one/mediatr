using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Client.LocalCommand;

public sealed record Command(string Value) : IRequest;