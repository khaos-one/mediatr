namespace Khaos.MediatR.Rpc.AspNetCore;

public record HttpEndpoint(string Route, Delegate RequestDelegate);