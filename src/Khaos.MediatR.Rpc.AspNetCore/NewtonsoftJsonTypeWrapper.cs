using System.Reflection;

using MediatR;

using Microsoft.AspNetCore.Http;

using Newtonsoft.Json;

namespace Khaos.MediatR.Rpc.AspNetCore;

public class NewtonsoftJsonTypeWrapper<T>
{
    public T? Value { get; }
    
    public NewtonsoftJsonTypeWrapper(T? inner)
    {
        Value = inner;  
    }

    public static async ValueTask<T?> BindAsync(HttpContext context, ParameterInfo parameterInfo)
    {
        if (!context.Request.HasJsonContentType())
        {
            throw new BadHttpRequestException(
                "Request content type was not a recognized JSON content type.",
                StatusCodes.Status415UnsupportedMediaType);
        }

        using var reader = new StreamReader(context.Request.Body);
        var stringContent = await reader.ReadToEndAsync();

        return JsonConvert.DeserializeObject<T>(stringContent);
    }
}