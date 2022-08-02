using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Contracts.TestWithoutReturnType;

public record Command(string Value) : IRequest;