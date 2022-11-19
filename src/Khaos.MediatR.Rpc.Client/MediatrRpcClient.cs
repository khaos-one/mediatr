using System.Text;
using System.Text.Json;

using Khaos.MediatR.Rpc.Codecs;

using MediatR;

namespace Khaos.MediatR.Rpc.Client;

public sealed class MediatrRpcClient<TMarker> : IMediatorRpcClient<TMarker>
{
    private static readonly string HttpClientName = ConfigurationGroupNameFactory.Get(typeof(TMarker));

    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new(JsonSerializerDefaults.Web);
    
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IStreamCodecFactory _streamCodecFactory;

    public MediatrRpcClient(IHttpClientFactory httpClientFactory, IStreamCodecFactory streamCodecFactory)
    {
        _httpClientFactory = httpClientFactory;
        _streamCodecFactory = streamCodecFactory;
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        var requestType = request.GetType();
        var streamCodec = _streamCodecFactory.GetOrDefault(typeof(TMarker));
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, TypeRoutePathFactory.Get(requestType));
        var requestString = await streamCodec.EncodeToString(request, cancellationToken);

        requestMessage.Content = new StringContent(
            requestString!,
            Encoding.UTF8,
            streamCodec.SupportedContentTypes.First());

        var client = _httpClientFactory.CreateClient(HttpClientName);
        var responseMessage = await client.SendAsync(requestMessage, cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new RpcClientException("Response status code does not indicate success.", responseMessage);
        }

        var response = await streamCodec.Decode(
            typeof(TResponse),
            await responseMessage.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken);

        if (response is null)
        {
            throw new InvalidOperationException("Response was null (deserialization error maybe?)");
        }

        return (TResponse) response!;
    }

    public Task<object?> Send(object request, CancellationToken cancellationToken = default) => 
        throw new NotSupportedException();

    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        throw new NotSupportedException();

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification => 
        throw new NotSupportedException();
}