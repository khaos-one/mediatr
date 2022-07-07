using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Contracts.TestWithoutReturnType;

public record Command(string Value) : IRequest;