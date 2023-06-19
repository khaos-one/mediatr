using System.Security.Claims;
using System.Text.Encodings.Web;

using MediatR;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Khaos.MediatR.Rpc.Sandbox.Server;

public sealed class StaticTokenAuthenticationScheme : AuthenticationHandler<StaticTokenAuthenticationSchemeOptions>
{
    public StaticTokenAuthenticationScheme(
        IOptionsMonitor<StaticTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(
        options,
        logger,
        encoder,
        clock)
    { }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }
}