using System.Text;

using MediatR;

using Newtonsoft.Json;

namespace Khaos.MediatrRpc.Client;

public sealed class MediatrRpcClient<TMarker> : IMediatorRpcClient<TMarker>
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
        var requestBody = JsonConvert.SerializeObject(request);
        requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        
        var responseMessage = await _client.SendAsync(requestMessage, cancellationToken);

        responseMessage.EnsureSuccessStatusCode();

        var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var response = JsonConvert.DeserializeObject<TResponse>(responseBody);

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