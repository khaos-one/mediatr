using Khaos.MediatR.Callable;
using Khaos.MediatR.Rpc;
using Khaos.MediatR.Rpc.AspNetCore;
using Khaos.MediatR.Rpc.Codecs;
using Khaos.MediatR.Rpc.Codecs.NewtosoftJson;using Khaos.MediatR.Rpc.Codecs.NewtosoftJson.AspNetCore;
using Khaos.MediatR.Rpc.Sandbox.Contracts;

using MediatR;

using AssemblyMarker = Khaos.MediatR.Rpc.Sandbox.Contracts.AssemblyMarker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(AssemblyMarker), typeof(Program));

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

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// Map MediatR RPC endpoints.
app.MapMediatR(typeof(AssemblyMarker));

// Add OpenAPI exploration support.
app.UseSwagger();
app.UseSwaggerUI();

app.Run();