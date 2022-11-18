using System.Text.Json;

using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore;

public sealed class SystemTextJsonStreamCodec : IStreamCodecAndMetadataEmitter
{
    private readonly JsonSerializerOptions? _options;

    public SystemTextJsonStreamCodec(JsonSerializerOptions? options = default)
    {
        _options = options;
    }

    public IReadOnlySet<string> SupportedContentTypes => new HashSet<string> {"application/json"};

    public ValueTask<object?> Decode(Type type, Stream stream, CancellationToken cancellationToken) =>
        JsonSerializer.DeserializeAsync(
            stream,
            type,
            _options,
            cancellationToken);

    public Task Encode(
        object? @object,
        Stream stream,
        CancellationToken cancellationToken) =>
        JsonSerializer.SerializeAsync(
            stream,
            @object,
            _options,
            cancellationToken);

    public IAcceptsMetadata CreateAcceptsMetadataForType(Type type) => new JsonAcceptsMetadata(type);
    public IProducesResponseTypeMetadata CreateProducesMetadataForType(Type type) => new JsonProducesMetadata(type);
}