using Khaos.MediatR.Rpc.Codecs;

using Microsoft.AspNetCore.Builder;

namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

public interface ICodecMetadataEmitter
{
    void EmitForType(IEndpointConventionBuilder builder, Type type);
}

public interface ICodecMetadataEmitter<T>
    : ICodecMetadataEmitter
    where T : IStreamCodec
{ }