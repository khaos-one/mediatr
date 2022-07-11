using Khaos.MediatrRpc.Client;
using Khaos.MediatrRpc.Sandbox.Contracts;
using Khaos.MediatrRpc.Sandbox.Contracts.Test;
using Khaos.MediatrRpc.Sandbox.Contracts.TestWithoutReturnType;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

services.AddMediatR(typeof(AssemblyMarker));
services.AddMediatrRpcClient(typeof(AssemblyMarker), options => options.BaseAddress = new Uri("http://localhost:5000"));

var serviceProvider = services.BuildServiceProvider();

var mediator = serviceProvider.GetRequiredService<IMediator>();
var result2 = await mediator.Send(new Query("hi again! "));

Console.WriteLine(result2.Value);

await mediator.Send(new Command("hi from client!"));

Console.WriteLine("Done.");