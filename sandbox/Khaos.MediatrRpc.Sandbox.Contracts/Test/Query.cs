using MediatR;

namespace Khaos.MediatrRpc.Sandbox.Contracts.Test;

public record Query(string Value) : IRequest<Result>;