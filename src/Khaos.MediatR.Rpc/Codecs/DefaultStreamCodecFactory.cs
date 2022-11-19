using System.Collections.Concurrent;

namespace Khaos.MediatR.Rpc.Codecs;

internal sealed class DefaultStreamCodecFactory : IStreamCodecFactory
{
    private readonly Dictionary<Type, IStreamCodec> _codecs = new();

    public void Add(Type markerType, IStreamCodec codec)
    {
        _codecs.Add(markerType, codec);
    }
    
    public IStreamCodec GetOrDefault(Type markerType)
    {
        if (_codecs.TryGetValue(markerType, out var codec))
        {
            return codec;
        }

        return GetDefault();
    }

    public IStreamCodec GetDefault() => _codecs[typeof(void)];
}