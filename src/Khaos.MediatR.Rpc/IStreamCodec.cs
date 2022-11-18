namespace Khaos.MediatR.Rpc;

public interface IStreamCodec
{
    IReadOnlySet<string> SupportedContentTypes { get; }

    ValueTask<object?> Decode(Type type, Stream stream, CancellationToken cancellationToken);
    Task Encode(object? @object, Stream stream, CancellationToken cancellationToken);
}