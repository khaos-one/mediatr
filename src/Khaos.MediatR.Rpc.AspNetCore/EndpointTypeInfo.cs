using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.AspNetCore;

public sealed record EndpointTypeInfo(Type MediatRType, Type MarkerType, IEndpointConventionBuilder EndpointConventionBuilder);