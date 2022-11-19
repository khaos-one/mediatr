namespace Khaos.MediatR.Rpc.Codecs;

public interface IStreamCodecFactory
{
    public IStreamCodec GetOrDefault(Type markerType);
    public IStreamCodec GetDefault();
}