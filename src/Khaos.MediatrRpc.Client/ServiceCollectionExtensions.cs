using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatrRpc.Client;

public static class ServiceCollectionExtensions
{
    public static void AddMediatrRpc<TMarker>(
        this IServiceCollection services)
    {
        AddMediatrRpcClient<TMarker>(services);
        AddMediatrTransboundaryHandlers(services, typeof(TMarker));
    }
    
    public static void AddMediatrRpc<TMarker>(
        this IServiceCollection services,
        Action<HttpClient> configureClient)
    {
        AddMediatrRpcClient<TMarker>(services, configureClient);
        AddMediatrTransboundaryHandlers(services, typeof(TMarker));
    }
    
    internal static IHttpClientBuilder AddMediatrRpcClient<TMarker>(
        this IServiceCollection services,
        Action<HttpClient> configureClient) => 
        services.AddHttpClient<IMediatorRpcClient<TMarker>, MediatrRpcClient<TMarker>>(configureClient);
    
    internal static IHttpClientBuilder AddMediatrRpcClient<TMarker>(
        this IServiceCollection services) => 
        services.AddHttpClient<IMediatorRpcClient<TMarker>, MediatrRpcClient<TMarker>>();

    internal static IServiceCollection AddMediatrTransboundaryHandlers(
        this IServiceCollection services, params Type[] assemblyMarkerTypes)
    {
        var genericHandlerWithReturnType = typeof(RpcRequestHandler<,>);
        var genericHandlerInterfaceWithReturnType = typeof(IRequestHandler<,>);
        var genericHandlerInterfaceWoReturnType = typeof(IRequestHandler<>);
        var transboundaryClientType = typeof(IMediatorRpcClient<>);

        foreach (var assemblyMarkerType in assemblyMarkerTypes)
        {
            var currentAssemblyTransboundaryClientType = transboundaryClientType.MakeGenericType(assemblyMarkerType);
            var assemblyDiscoverer = new MediatrAssemblyDiscoverer(new[] {assemblyMarkerType});
            
            foreach (var mediatrType in assemblyDiscoverer.EnumerateMediatrTypes())
            {
                var returnType = RequestReturnTypeExtractor.TryGetReturnType(mediatrType);

                Type handlerInterfaceType;
                Type handlerType;
                
                if (returnType is not null)
                {
                    handlerInterfaceType =
                        genericHandlerInterfaceWithReturnType.MakeGenericType(mediatrType, returnType);
                    handlerType = genericHandlerWithReturnType.MakeGenericType(mediatrType, returnType);
                }
                else
                {
                    handlerInterfaceType = genericHandlerInterfaceWoReturnType.MakeGenericType(mediatrType);
                    handlerType = genericHandlerWithReturnType.MakeGenericType(mediatrType, typeof(Unit));
                }
                
                services.AddScoped(
                    handlerInterfaceType,
                    sp =>
                    {
                        var transboundaryClient =
                            (IMediatorRpcClient) sp.GetRequiredService(currentAssemblyTransboundaryClientType);

                        return Activator.CreateInstance(handlerType, transboundaryClient)!;
                    });
            }
        }

        return services;
    }
}