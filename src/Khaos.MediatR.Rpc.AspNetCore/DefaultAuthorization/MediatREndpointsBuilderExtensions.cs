using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.AspNetCore.DefaultAuthorization;

public static class MediatREndpointsBuilderExtensions
{
    public static IMediatREndpointsBuilder RequireAuthorizationPolicies(
        this IMediatREndpointsBuilder builder,
        params string[] policiesNames)
    {
        foreach (var endpointInfo in builder.EnumerateEndpoints())
        {
            endpointInfo.EndpointConventionBuilder.RequireAuthorization(policiesNames);
        }

        return builder;
    }

    public static IMediatREndpointsBuilder WithCustomEndpointForType(
        this IMediatREndpointsBuilder builder,
        Type type,
        Action<IEndpointConventionBuilder> endpointBuilder)
    {
        foreach (var endpointInfo in builder.EnumerateEndpoints())
        {
            if (endpointInfo.MediatRType == type)
            {
                endpointBuilder(endpointInfo.EndpointConventionBuilder);
            }
        }

        return builder;
    }
}