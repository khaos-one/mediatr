using Khaos.MediatR.Rpc.Codecs;

namespace Khaos.MediatR.Rpc.Client;

internal static class StreamCodecStringExtensions
{
    public static async Task<string?> EncodeToString(
        this IStreamCodec streamCodec,
        object? @object,
        CancellationToken cancellationToken)
    {
        if (@object is null)
        {
            return null;
        }
        
        using var ms = new MemoryStream();

        await streamCodec.Encode(@object, ms, cancellationToken);

        ms.Seek(0, SeekOrigin.Begin);
        using var sr = new StreamReader(ms);

        return await sr.ReadToEndAsync();
    }
}