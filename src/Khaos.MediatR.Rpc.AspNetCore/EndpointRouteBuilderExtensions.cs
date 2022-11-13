using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Khaos.MediatR.Rpc.AspNetCore;

public static class EndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapMediatR(
        this IEndpointRouteBuilder endpointRouteBuilder,
        Type[] assembliesMarkerTypes,
        IHttpDelegateFactory httpDelegateFactory)
    {
        var discoverer = new MediatrAssemblyDiscoverer(assembliesMarkerTypes);
        var httpEndpointBuilder = new HttpEndpointsBuilder(discoverer, httpDelegateFactory);

        foreach (var ep in httpEndpointBuilder.EnumerateEndpoints())
        {
            var routeBuilder = endpointRouteBuilder.MapPost(ep.Route, ep.RequestDelegate);
            ep.AdditionalRouteConfigurator?.Invoke(routeBuilder);
        }

        return endpointRouteBuilder;
    }

    public static IEndpointRouteBuilder MapMediatR(
        this IEndpointRouteBuilder endpointRouteBuilder,
        params Type[] assembliesMarkerTypes) =>
        MapMediatR(endpointRouteBuilder, assembliesMarkerTypes, new PlainHttpDelegateFactory());

    public static IEndpointRouteBuilder MapMediatRWithNewtonsoftJson(
        this IEndpointRouteBuilder endpointRouteBuilder,
        params Type[] assembliesMarkerTypes) =>
        MapMediatR(endpointRouteBuilder, assembliesMarkerTypes, new NewtonsoftJsonHttpDelegateFactory());
}