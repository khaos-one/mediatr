using Khaos.MediatR.Rpc.AspNetCore;
using Khaos.MediatR.Rpc.Sandbox.Contracts;

using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(AssemblyMarker), typeof(Program));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapMediatR(typeof(AssemblyMarker));

app.Run();