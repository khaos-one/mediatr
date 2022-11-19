using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

public interface ICodecMetadataEmitter
{
    IAcceptsMetadata GetAcceptsMetadataForType(Type type);
    IProducesResponseTypeMetadata GetProducesMetadataForType(Type type);
}

public interface ICodecMetadataEmitter<T>
    : ICodecMetadataEmitter
    where T : IStreamCodec
{ }