using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Khaos.MediatR.Rpc.Codecs;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultStreamCodec(this IServiceCollection services, IStreamCodec codec)
    {
        var codecFactory = GetDefaultFactory(services);
        codecFactory.Add(typeof(void), codec);

        return services;
    }
    
    public static IServiceCollection AddStreamCodec(this IServiceCollection services, Type markerType, IStreamCodec codec)
    {
        var codecFactory = GetDefaultFactory(services);
        codecFactory.Add(markerType, codec);

        return services;
    }

    private static DefaultStreamCodecFactory GetDefaultFactory(IServiceCollection services)
    {
        services.TryAddSingleton<IStreamCodecFactory>(new DefaultStreamCodecFactory());
        var codecFactoryDescriptor = services.Single(sd => sd.ServiceType == typeof(IStreamCodecFactory));
        
        return (DefaultStreamCodecFactory) codecFactoryDescriptor.ImplementationInstance!;
    }
}