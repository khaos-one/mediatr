using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

public sealed class SystemTextJsonCodecMetadataEmitter : ICodecMetadataEmitter<SystemTextJsonStreamCodec>
{
    public IAcceptsMetadata GetAcceptsMetadataForType(Type type) => new JsonAcceptsMetadata(type);
    public IProducesResponseTypeMetadata GetProducesMetadataForType(Type type) => new JsonProducesMetadata(type);
}