using System.Text.Json;

namespace Khaos.MediatR.Rpc.Codecs;

public class SystemTextJsonStreamCodec : IStreamCodec
{
    private readonly JsonSerializerOptions? _options;

    public SystemTextJsonStreamCodec(JsonSerializerOptions? options = default)
    {
        _options = options;
    }

    public IReadOnlySet<string> SupportedContentTypes => new HashSet<string> {"application/json"};
    public string OutputContentType => "application/json; charset=utf-8";

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
}