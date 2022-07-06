// See https://aka.ms/new-console-template for more information

using Khaos.MediatrRpc.Client;
using Khaos.MediatrRpc.Sandbox.Contracts;
using Khaos.MediatrRpc.Sandbox.Contracts.Test;

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("http://localhost:5000");

var client = new MediatrRpcClient<AssemblyMarker>(httpClient);

var result = await client.Send(new Query("Hi"));

Console.WriteLine("Done!");