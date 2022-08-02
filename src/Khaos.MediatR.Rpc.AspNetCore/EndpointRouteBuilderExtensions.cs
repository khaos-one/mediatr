using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMediatR(
        this IEndpointRouteBuilder routeBuilder,
        params Type[] assembliesMarkerTypes)
    {
        var discoverer = new MediatrAssemblyDiscoverer(assembliesMarkerTypes);
        var httpEndpointBuilder = new HttpEndpointsBuilder(discoverer);

        foreach (var ep in httpEndpointBuilder.EnumerateEndpoints())
        {
            routeBuilder.MapPost(ep.Route, ep.RequestDelegate);
        }

        return routeBuilder;
    }
}