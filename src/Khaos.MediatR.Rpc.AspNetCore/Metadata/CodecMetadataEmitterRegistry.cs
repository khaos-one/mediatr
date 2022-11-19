namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

internal class CodecMetadataEmitterRegistry
{
    private readonly Dictionary<Type, ICodecMetadataEmitter> _registry = new();

    public void Add(Type codecType, ICodecMetadataEmitter metadataEmitter)
    {
        _registry.Add(codecType, metadataEmitter);
    }

    public ICodecMetadataEmitter Get(Type codecType) => _registry[codecType];
}