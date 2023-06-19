using Khaos.MediatR.Rpc.AspNetCore.Metadata;
using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IMediatREndpointsBuilder MapMediatR(
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

        var endpoints = httpEndpointBuilder.EnumerateAndBuild(endpointRouteBuilder);
        var endpointsBuilder = new DefaultMediatREndpointsBuilder(endpoints);

        return endpointsBuilder;
    }
}