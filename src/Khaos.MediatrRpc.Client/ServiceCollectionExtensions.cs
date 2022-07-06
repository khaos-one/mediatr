using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatrRpc.Client;

public static class ServiceCollectionExtensions
{
    public static IHttpClientBuilder AddMediatrRpcClient<TMarker>(
        this IServiceCollection services,
        Action<HttpClient> configureClient) => 
        services.AddHttpClient<IMediatorRpcClient<TMarker>, MediatrRpcClient<TMarker>>(configureClient);
    
    public static IHttpClientBuilder AddMediatrRpcClient<TMarker>(
        this IServiceCollection services) => 
        services.AddHttpClient<IMediatorRpcClient<TMarker>, MediatrRpcClient<TMarker>>();
}