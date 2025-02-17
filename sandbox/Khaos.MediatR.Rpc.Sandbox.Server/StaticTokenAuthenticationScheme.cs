using System.Security.Claims;
using System.Text.Encodings.Web;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Khaos.MediatR.Rpc.Sandbox.Server;

public sealed class StaticTokenAuthenticationScheme 
    : AuthenticationHandler<StaticTokenAuthenticationSchemeOptions>
{
    public StaticTokenAuthenticationScheme(
        IOptionsMonitor<StaticTokenAuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder) : base(
        options,
        logger,
        encoder)
    { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[] { new Claim(ClaimTypes.Name, "Test") };
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "Tokens"));
        var ticket = new AuthenticationTicket(principal, this.Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}