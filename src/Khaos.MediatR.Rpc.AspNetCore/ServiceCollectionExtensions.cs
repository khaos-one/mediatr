using System.Reflection;

using Khaos.MediatR.Rpc.AspNetCore.Metadata;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMediatRRpcAspNetCore(this IServiceCollection services)
    {
        var registry = GetCodecMetadataEmitterRegistry(services);

        foreach (var (codecType, metadataEmitter) in EnumerateMetadataEmittersForCodecs())
        {
            registry.Add(codecType, metadataEmitter);
        }

        return services;
    }
    
    private static CodecMetadataEmitterRegistry GetCodecMetadataEmitterRegistry(IServiceCollection services)
    {
        services.TryAddSingleton(new CodecMetadataEmitterRegistry());
        var codecFactoryDescriptor = services.Single(sd => sd.ServiceType == typeof(CodecMetadataEmitterRegistry));
        
        return (CodecMetadataEmitterRegistry) codecFactoryDescriptor.ImplementationInstance!;
    }

    private static IEnumerable<(Type CodecType, ICodecMetadataEmitter MetadataEmitter)>
        EnumerateMetadataEmittersForCodecs()
    {
        var suitableTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.ExportedTypes)
            .Where(
                type => !type.IsAbstract
                    && !type.IsInterface
                    && type.IsAssignableTo(typeof(ICodecMetadataEmitter)));
        
        foreach (var emitterType in suitableTypes)
        {
            var codecInterfaceType = emitterType.GetInterface("ICodecMetadataEmitter`1");

            if (codecInterfaceType is null)
            {
                continue;
            }
            
            var codecType = codecInterfaceType.GetGenericArguments().First();
            var instance = Activator.CreateInstance(emitterType);

            if (instance is null)
            {
                throw new Exception(
                    $"Failed to create instance of codec metadata emitter type {emitterType.FullName}.");
            }

            yield return (codecType, (ICodecMetadataEmitter) instance);
        }
    }
}