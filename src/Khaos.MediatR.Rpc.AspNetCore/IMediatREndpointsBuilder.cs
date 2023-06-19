namespace Khaos.MediatR.Rpc.AspNetCore;

public interface IMediatREndpointsBuilder
{ 
    IEnumerable<EndpointTypeInfo> EnumerateEndpoints();
}