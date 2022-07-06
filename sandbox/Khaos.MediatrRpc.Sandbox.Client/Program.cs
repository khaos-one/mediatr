using Khaos.MediatrRpc.Client;
using Khaos.MediatrRpc.Sandbox.Contracts;
using Khaos.MediatrRpc.Sandbox.Contracts.Test;

using Microsoft.Extensions.DependencyInjection;

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5000");

var client = new MediatrRpcClient<AssemblyMarker>(httpClient);

var result = await client.Send(new Query("Hi"));

Console.WriteLine(result.Value);

var services = new ServiceCollection();
services.AddMediatrRpcClient<AssemblyMarker>(options => options.BaseAddress = new Uri("http://localhost:5000"));

var serviceProvider = services.BuildServiceProvider();

var mediatorRpcClient = serviceProvider.GetRequiredService<IMediatorRpcClient<AssemblyMarker>>();
var result2 = await mediatorRpcClient.Send(new Query("hi again! "));

Console.WriteLine(result2.Value);