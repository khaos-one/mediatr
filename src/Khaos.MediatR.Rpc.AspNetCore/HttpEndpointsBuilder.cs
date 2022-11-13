using Microsoft.AspNetCore.Http;

namespace Khaos.MediatR.Rpc.AspNetCore;

public class HttpEndpointsBuilder
{
    private readonly MediatrAssemblyDiscoverer _discoverer;
    private readonly IHttpDelegateFactory _delegateFactory;

    public HttpEndpointsBuilder(MediatrAssemblyDiscoverer discoverer, IHttpDelegateFactory delegateFactory)
    {
        _discoverer = discoverer;
        _delegateFactory = delegateFactory;
    }

    public IEnumerable<HttpEndpoint> EnumerateEndpoints()
    {
        foreach (var mediatrType in _discoverer.EnumerateMediatrTypes())
        {
            var delegateInfo = _delegateFactory.Construct(mediatrType);
            
            yield return new(
                TypeRoutePathFactory.Get(mediatrType),
                delegateInfo.Delegate,
                routeBuilder => 
                    routeBuilder.Produces(StatusCodes.Status200OK, delegateInfo.VisibleReturnType, "application/json"));
        }
    }
}