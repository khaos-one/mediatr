using Khaos.MediatR.Callable;
using Khaos.MediatR.Rpc.AspNetCore;
using Khaos.MediatR.Rpc.Sandbox.Contracts;

using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(AssemblyMarker), typeof(Program));
builder.Services.AddMediatRCallables();

// OpenAPI exploring configuration.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapMediatRWithNewtonsoftJson(typeof(AssemblyMarker));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();