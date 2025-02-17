using MediatR;

namespace Khaos.MediatR.Rpc.Client;

public sealed class RpcRequestHandler<TRequest, TResponse>(IMediatorRpcClient client)
    : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken) =>
        client.Send(request, cancellationToken);
}

public sealed class RpcRequestHandler<TRequest>(IMediatorRpcClient client)
    : IRequestHandler<TRequest>
    where TRequest : IRequest
{
    public Task Handle(TRequest request, CancellationToken cancellationToken) =>
        client.Send(request, cancellationToken);
}