using System.Text;
using System.Text.Json;

using MediatR;

using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Khaos.MediatrRpc.Client;

public sealed class MediatrRpcClient<TMarker> : IMediatorRpcClient<TMarker>
{
    private static readonly string HttpClientName = HttpClientNameFactory.Get(typeof(TMarker));

    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IHttpClientFactory _httpClientFactory;

    public MediatrRpcClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, TypeRoutePathFactory.Get(requestType));
        var requestBody = JsonSerializer.Serialize(
            request,
            requestType,
            DefaultJsonSerializerOptions);

        requestMessage.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var responseMessage = await client.SendAsync(requestMessage, cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new RpcClientException("Response status code does not indicate success.", responseMessage);
        }

        var responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
        var response = JsonSerializer.Deserialize<TResponse>(responseBody, DefaultJsonSerializerOptions);

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