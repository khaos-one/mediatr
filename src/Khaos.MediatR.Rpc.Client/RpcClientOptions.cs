namespace Khaos.MediatR.Rpc.Client;

public sealed class RpcClientOptions
{
    public Action<HttpClient>? ConfigureHttpClient { get; set; }
    public ICollection<Type> CommonPipelineBehaviours { get; } = new List<Type>();
}