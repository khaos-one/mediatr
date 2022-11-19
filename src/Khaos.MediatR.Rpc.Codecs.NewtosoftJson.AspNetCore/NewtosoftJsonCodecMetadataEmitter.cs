using Khaos.MediatR.Rpc.AspNetCore.Metadata;

using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.Codecs.NewtosoftJson.AspNetCore;

public sealed class NewtosoftJsonCodecMetadataEmitter : ICodecMetadataEmitter<NewtosoftJsonStreamCodec>
{
    public void EmitForType(IEndpointConventionBuilder builder, Type type)
    {
        builder
            .WithMetadata(new JsonAcceptsMetadata(type))
            .WithMetadata(new JsonProducesMetadata(RequestReturnTypeExtractor.TryGetReturnType(type)!));
    }
}