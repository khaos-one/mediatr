using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Client.LocalCommand;

public sealed record Command(string Value) : IRequest;