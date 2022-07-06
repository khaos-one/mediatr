using Microsoft.AspNetCore.Http;

namespace Khaos.MediatrRpc.AspNetCore;

public record HttpEndpoint(string Route, Delegate RequestDelegate);