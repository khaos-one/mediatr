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
    private readonly IStreamCodec _streamCodec;

    private readonly ClientTypesCache _cache = new();

    public MediatrRpcClient(IHttpClientFactory httpClientFactory, IStreamCodecFactory streamCodecFactory)
    {
        _httpClientFactory = httpClientFactory;
        _streamCodec = streamCodecFactory.GetOrDefault(typeof(TMarker))
            ?? throw new ArgumentException(
                $"There is no stream codec registered for marker type {typeof(TMarker).FullName}.",
                nameof(streamCodecFactory));
    }

    private async Task<object?> SendInternal(
        object request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var cachedTypeInfo = _cache.GetOrCreateForType(requestType);

        if (cachedTypeInfo.ResponseType is null)
        {
            throw new ArgumentException(
                "Provided request type has no IRequest<TResponse> interface, therefore response type cannot be inferred.",
                nameof(request));
        }
        
        var requestMessage = new HttpRequestMessage(HttpMethod.Post, cachedTypeInfo.RoutePath);
        var requestString = await _streamCodec.EncodeToString(request, cancellationToken);

        requestMessage.Content = new StringContent(
            requestString!,
            Encoding.UTF8,
            _streamCodec.SupportedContentTypes.First());
        
        var client = _httpClientFactory.CreateClient(HttpClientName);
        var responseMessage = await client.SendAsync(requestMessage, cancellationToken);

        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new RpcClientException(
                $"Response status code ({responseMessage.StatusCode}) does not indicate success.",
                responseMessage);
        }

        if (cachedTypeInfo.ResponseType == typeof(Unit))
        {
            return default;
        }
        
        var response = await _streamCodec.Decode(
            cachedTypeInfo.ResponseType,
            await responseMessage.Content.ReadAsStreamAsync(cancellationToken),
            cancellationToken);
        
        if (response is null)
        {
            throw new InvalidOperationException("Response was null (deserialization or network error maybe?)");
        }

        return  response;
    }

    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = new()) =>
        (TResponse) (await SendInternal(request, cancellationToken)
            ?? throw new InvalidOperationException("Response type was null (sic!)."));

    public Task Send<TRequest>(TRequest request, CancellationToken cancellationToken = new())
        where TRequest : IRequest =>
        SendInternal(request, cancellationToken);

    public Task<object?> Send(object request, CancellationToken cancellationToken = new()) =>
        SendInternal(request, cancellationToken);

    public Task Publish(object notification, CancellationToken cancellationToken = default) =>
        SendInternal(notification, cancellationToken);

    public Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification =>
        SendInternal(notification, cancellationToken);

    public IAsyncEnumerable<TResponse> CreateStream<TResponse>(
        IStreamRequest<TResponse> request, CancellationToken cancellationToken = new()) => 
        throw new NotSupportedException("Async streams are not yet supported by RPC.");

    public IAsyncEnumerable<object?> CreateStream(object request, CancellationToken cancellationToken = new()) => 
        throw new NotSupportedException("Async streams are not yet supported by RPC.");
}