using MediatR;

using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.Client;

public static class ServiceCollectionExtensions
{
    public static void AddMediatRRpcClient(
        this IServiceCollection services,
        Type assemblyMarkerType,
        Action<RpcClientOptions> configureOptions)
    {
        var options = new RpcClientOptions();

        configureOptions(options);
        
        AddMediatRRpcHttpClient(services, assemblyMarkerType, options.ConfigureHttpClient);
        AddTransboundaryHandlers(services, options, assemblyMarkerType);
    }

    private static void AddMediatRRpcHttpClient(
        IServiceCollection services,
        Type assemblyMarkerType,
        Action<HttpClient>? configureHttpClient)
    {
        if (configureHttpClient is not null)
        {
            services.AddHttpClient(HttpClientNameFactory.Get(assemblyMarkerType), configureHttpClient);
        }
        else
        {
            services.AddHttpClient(HttpClientNameFactory.Get(assemblyMarkerType));
        }

        services.AddScoped(
            typeof(IMediatorRpcClient<>).MakeGenericType(assemblyMarkerType),
            typeof(MediatrRpcClient<>).MakeGenericType(assemblyMarkerType));
    }
    
    private static void AddTransboundaryHandlers(
        IServiceCollection services,
        RpcClientOptions options,
        params Type[] assemblyMarkerTypes)
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

                if (options.CommonPipelineBehaviours.Any())
                {
                    AddCommonPipelineBehavioursForType(
                        services,
                        mediatrType,
                        returnType,
                        options.CommonPipelineBehaviours);
                }
            }
        }
    }
    
    private static void AddCommonPipelineBehavioursForType(
        IServiceCollection services,
        Type requestType,
        Type? responseType,
        ICollection<Type> pipelineBehaviourTypes)
    {
        foreach (var behaviourType in pipelineBehaviourTypes)
        {
            if (!behaviourType.IsGenericType || behaviourType.GetInterface("IPipelineBehavior`2") is null)
            {
                throw new ArgumentException(
                    "Provided common RPC pipeline behaviour must be generic type inherited from IPipelineBehaviour`2");
            }

            var concreteBehaviour = behaviourType.MakeGenericType(requestType, responseType ?? typeof(Unit));
            var concreteBehaviourInterface =
                typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType ?? typeof(Unit));

            services.AddTransient(concreteBehaviourInterface, concreteBehaviour);
        }
    }

}