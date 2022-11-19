namespace Khaos.MediatR.Rpc.Codecs;

public interface IStreamCodec
{
    IReadOnlySet<string> SupportedContentTypes { get; }
    string OutputContentType { get; }

    ValueTask<object?> Decode(Type type, Stream stream, CancellationToken cancellationToken);
    Task Encode(object? @object, Stream stream, CancellationToken cancellationToken);
}