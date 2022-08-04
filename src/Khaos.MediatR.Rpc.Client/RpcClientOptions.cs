using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.Client;

public sealed class RpcClientOptions
{
    public Action<IHttpClientBuilder>? ConfigureHttpClient { get; set; }
    public ICollection<Type> CommonPipelineBehaviours { get; } = new List<Type>();
}