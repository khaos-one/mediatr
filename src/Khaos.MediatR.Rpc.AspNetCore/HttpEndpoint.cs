using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.AspNetCore;

public record HttpEndpoint(
    string Route, 
    Delegate RequestDelegate, 
    Action<RouteHandlerBuilder>? AdditionalRouteConfigurator = default);