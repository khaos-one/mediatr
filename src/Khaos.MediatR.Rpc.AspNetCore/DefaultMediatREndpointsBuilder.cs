using System.Collections.Immutable;

namespace Khaos.MediatR.Rpc.AspNetCore;

internal sealed class DefaultMediatREndpointsBuilder : IMediatREndpointsBuilder
{
    private readonly IReadOnlyDictionary<Type, EndpointTypeInfo> _registeredEndpoints;

    public DefaultMediatREndpointsBuilder(IEnumerable<EndpointTypeInfo> mediatrEndpoints)
    {
        _registeredEndpoints = mediatrEndpoints.ToImmutableDictionary(k => k.MediatRType, v => v);
    }

    public IEnumerable<EndpointTypeInfo> EnumerateEndpoints() => _registeredEndpoints.Values;
}