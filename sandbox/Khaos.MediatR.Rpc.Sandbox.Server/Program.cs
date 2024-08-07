using System.Security.Claims;

using Khaos.MediatR.Callable;
using Khaos.MediatR.Rpc;
using Khaos.MediatR.Rpc.AspNetCore;
using Khaos.MediatR.Rpc.AspNetCore.DefaultAuthorization;
using Khaos.MediatR.Rpc.Codecs;
using Khaos.MediatR.Rpc.Codecs.NewtosoftJson;
using Khaos.MediatR.Rpc.Sandbox.Server;

using AssemblyMarker = Khaos.MediatR.Rpc.Sandbox.Contracts.AssemblyMarker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(
    cfg => cfg
        .RegisterServicesFromAssemblyContaining<AssemblyMarker>()
        .RegisterServicesFromAssemblyContaining<Program>());

// Configure stream codec for assembly and add needed Rpc.AspNetCore services.
Touch.Assembly(typeof(Khaos.MediatR.Rpc.Codecs.NewtosoftJson.AspNetCore.AssemblyMarker));

builder.Services.AddStreamCodec(
    typeof(AssemblyMarker),
    new NewtosoftJsonStreamCodec());
builder.Services.AddMediatRRpcAspNetCore();

// Configure MediatR callables.
builder.Services.AddMediatRCallables();

// OpenAPI exploring configuration.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddAuthentication("StaticToken")
//     .AddScheme<StaticTokenAuthenticationSchemeOptions, StaticTokenAuthenticationScheme>("StaticToken", opts => { });
// builder.Services.AddAuthorization(
//     conf =>
//     {
//         conf.AddPolicy("admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Name, "Test"));
//         conf.AddPolicy("non-passable-policy", policyBuilder => policyBuilder.RequireClaim("non-existent-claim", "non-existent-value"));
//     });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.UseAuthentication();
// app.UseAuthorization();

// Map MediatR RPC endpoints.
app.MapMediatR(typeof(AssemblyMarker))
    ;
    // .RequireAuthorizationPolicies("admin")
    // .WithCustomEndpointForType(
    //     typeof(Khaos.MediatR.Rpc.Sandbox.Contracts.TestWithoutReturnType.Command),
    //     endpointBuilder => endpointBuilder.RequireAuthorization("non-passable-policy"));

// Add OpenAPI exploration support.
app.UseSwagger();
app.UseSwaggerUI();

app.Run();