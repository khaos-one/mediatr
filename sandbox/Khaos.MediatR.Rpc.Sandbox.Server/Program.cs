using System.Text.Json;

using Khaos.MediatR.Callable;
using Khaos.MediatR.Rpc.AspNetCore;
using Khaos.MediatR.Rpc.Sandbox.Contracts;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(AssemblyMarker), typeof(Program));
builder.Services.AddMediatRCallables();

// OpenAPI exploring configuration.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapMediatR(
    new[] {typeof(AssemblyMarker)},
    new SystemTextJsonStreamCodec(
        new JsonSerializerOptions(JsonSerializerDefaults.Web)));

app.UseSwagger();
app.UseSwaggerUI();

app.Run();