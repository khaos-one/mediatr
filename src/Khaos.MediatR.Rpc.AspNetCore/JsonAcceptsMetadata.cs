using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore;

internal sealed class JsonAcceptsMetadata : IAcceptsMetadata
{
    public IReadOnlyList<string> ContentTypes => new[] {"application/json"};
    public bool IsOptional => false;
    
    public Type? RequestType { get; }

    public JsonAcceptsMetadata(Type requestType)
    {
        RequestType = requestType;
    }
}