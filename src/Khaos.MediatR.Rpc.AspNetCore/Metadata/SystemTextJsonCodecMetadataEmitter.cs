using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

public sealed class SystemTextJsonCodecMetadataEmitter : ICodecMetadataEmitter<SystemTextJsonStreamCodec>
{
    public void EmitForType(IEndpointConventionBuilder builder, Type type)
    {
        builder
            .WithMetadata(new JsonAcceptsMetadata(type))
            .WithMetadata(new JsonProducesMetadata(RequestReturnTypeExtractor.TryGetReturnType(type)!));
    }
}