using Khaos.MediatR.Rpc.AspNetCore.Metadata;
using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMediatR(
        this IEndpointRouteBuilder endpointRouteBuilder,
        params Type[] assembliesMarkerTypes)
    {
        var streamCodecFactory = endpointRouteBuilder.ServiceProvider.GetRequiredService<IStreamCodecFactory>();
        var codecMetadataEmitterRegistry =
            endpointRouteBuilder.ServiceProvider.GetRequiredService<CodecMetadataEmitterRegistry>();
        
        var discoverer = new MediatrAssemblyDiscoverer(assembliesMarkerTypes);
        var httpEndpointBuilder = new HttpEndpointsBuilder(
            discoverer,
            streamCodecFactory,
            codecMetadataEmitterRegistry);

        httpEndpointBuilder.Build(endpointRouteBuilder);
        
        return endpointRouteBuilder;
    }
}