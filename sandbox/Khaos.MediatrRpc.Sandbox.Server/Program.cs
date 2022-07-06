using Khaos.MediatrRpc.AspNetCore;
using Khaos.MediatrRpc.Sandbox.Contracts;
using MediatR;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(AssemblyMarker), typeof(Program));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapMediatR(typeof(AssemblyMarker));

app.Run();