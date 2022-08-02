using Khaos.MediatR.Rpc.Client;
using Khaos.MediatR.Rpc.Sandbox.Client;
using Khaos.MediatR.Rpc.Sandbox.Contracts;
using Khaos.MediatR.Rpc.Sandbox.Contracts.Test;
using Khaos.MediatR.Rpc.Sandbox.Contracts.TestWithoutReturnType;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

// Add local commands.
services.AddMediatR(typeof(Khaos.MediatR.Rpc.Sandbox.Client.LocalCommand.Command));

// Add remote commands and a client.
services.AddMediatR(typeof(AssemblyMarker));
services.AddMediatrRpcClient(typeof(AssemblyMarker), options =>
{
    options.ConfigureHttpClient = client => client.BaseAddress = new Uri("http://localhost:5000");
    options.CommonPipelineBehaviours.Add(typeof(LoggingPipelineBehaviour<,>));
});

var serviceProvider = services.BuildServiceProvider();

// Test remote commands/queries.
var mediator = serviceProvider.GetRequiredService<IMediator>();
var result2 = await mediator.Send(new Query("hi again! "));

Console.WriteLine(result2.Value);

await mediator.Send(new Command("hi from client!"));

// Test local commands/queries.
await mediator.Send(new Khaos.MediatR.Rpc.Sandbox.Client.LocalCommand.Command("local command invocation"));

Console.WriteLine("Done.");