using System.Net.Http.Json;
using System.Text.Json;
using MediatR;

namespace Khaos.MediatrRpc.Client;

public sealed class MediatrRpcClient<TMarker> : IMediatorRpcClient
{
    private readonly HttpClient _client;

    public MediatrRpcClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, TypeRouteNameFactory.Get(requestType));
        requestMessage.Content = JsonContent.Create(
            request, 
            options: new JsonSerializerOptions(JsonSerializerDefaults.Web));

        var responseMessage = await _client.SendAsync(requestMessage, cancellationToken);

        responseMessage.EnsureSuccessStatusCode();

        var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var response = JsonSerializer.Deserialize<TResponse>(
            responseBody, 
            new JsonSerializerOptions(JsonSerializerDefaults.Web));

        if (response is null)
        {
            throw new InvalidOperationException("Response was null (deserialization error maybe?)");
        }

        return response!;
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default) => 
        throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification => 
        throw new NotSupportedException();
}