using MediatR;

namespace Khaos.MediatrRpc.Client;

public sealed class RpcRequestHandler<TRequest, TResponse> 
    : IRequestHandler<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IMediatorRpcClient _client;

    public RpcRequestHandler(IMediatorRpcClient client)
    {
        _client = client;
    }

    public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken) =>
        _client.Send(request, cancellationToken);
}