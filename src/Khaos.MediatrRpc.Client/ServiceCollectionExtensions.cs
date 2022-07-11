using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatrRpc.Client;

public static class ServiceCollectionExtensions
{
    public static void AddMediatrRpcClient(
        this IServiceCollection services,
        Type assemblyMarkerType,
        Action<HttpClient> configureHttpClient)
    {
        AddMediatrRpcHttpClient(services, assemblyMarkerType, configureHttpClient);
        AddMediatrRpcTransboundaryHandlers(services, assemblyMarkerType);
    }

    private static void AddMediatrRpcHttpClient(
        this IServiceCollection services,
        Type assemblyMarkerType,
        Action<HttpClient> configureHttpClient)
    {
        services.AddHttpClient(HttpClientNameFactory.Get(assemblyMarkerType), configureHttpClient);
        services.AddScoped(
            typeof(IMediatorRpcClient<>).MakeGenericType(assemblyMarkerType),
            typeof(MediatrRpcClient<>).MakeGenericType(assemblyMarkerType));
    }

    private static void AddMediatrRpcTransboundaryHandlers(
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
    }
}