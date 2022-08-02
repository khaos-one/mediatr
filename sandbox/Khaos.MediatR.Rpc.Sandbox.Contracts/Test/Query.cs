using MediatR;

namespace Khaos.MediatR.Rpc.Sandbox.Contracts.Test;

public record Query(string Value) : IRequest<Result>;