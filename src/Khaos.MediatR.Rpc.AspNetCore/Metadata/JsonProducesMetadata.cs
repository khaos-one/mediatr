using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Metadata;

namespace Khaos.MediatR.Rpc.AspNetCore.Metadata;

internal sealed class JsonProducesMetadata : IProducesResponseTypeMetadata
{
    public IEnumerable<string> ContentTypes => new[] {"application/json"};
    public int StatusCode => StatusCodes.Status200OK;
    
    public Type? Type { get; }

    public JsonProducesMetadata(Type type)
    {
        Type = type;
    }
}