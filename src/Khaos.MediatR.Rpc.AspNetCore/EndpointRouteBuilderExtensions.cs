using Microsoft.AspNetCore.Routing;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMediatR(
        this IEndpointRouteBuilder endpointRouteBuilder,
        Type[] assembliesMarkerTypes,
        IStreamCodecAndMetadataEmitter httpStreamCodec)
    {
        var discoverer = new MediatrAssemblyDiscoverer(assembliesMarkerTypes);
        var httpEndpointBuilder = new HttpEndpointsBuilder(discoverer, httpStreamCodec);

        httpEndpointBuilder.Build(endpointRouteBuilder);
        
        return endpointRouteBuilder;
    }
}