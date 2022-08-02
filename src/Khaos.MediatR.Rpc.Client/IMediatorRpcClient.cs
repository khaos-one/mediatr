using MediatR;

namespace Khaos.MediatR.Rpc.Client;

public interface IMediatorRpcClient : IMediator
{ }

public interface IMediatorRpcClient<TMarker> : IMediatorRpcClient
{ }