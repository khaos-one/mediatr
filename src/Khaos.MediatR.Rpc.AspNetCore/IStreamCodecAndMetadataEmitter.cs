using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore;

public interface IStreamCodecAndMetadataEmitter : IStreamCodec
{
    IAcceptsMetadata CreateAcceptsMetadataForType(Type type);
    IProducesResponseTypeMetadata CreateProducesMetadataForType(Type type);
}