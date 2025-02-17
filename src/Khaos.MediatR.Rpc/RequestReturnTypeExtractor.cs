using MediatR;

namespace Khaos.MediatR.Rpc;

public static class RequestReturnTypeExtractor
{
    public static Type? TryGetReturnType(Type mediatrType)
    {
        if (mediatrType.IsGenericType && mediatrType.GetGenericTypeDefinition() == typeof(IRequest<>))
        {
            return mediatrType.GetGenericArguments().Single();
        }

        if (mediatrType == typeof(IRequest) || mediatrType == typeof(INotification))
        {
            return typeof(void);
        }
        
        var responseType = mediatrType
            .GetInterfaces()
            .Where(
                interfaceType => 
                    interfaceType.IsGenericType
                    && interfaceType.GetGenericTypeDefinition() == typeof(IRequest<>))
            .Select(interfaceType => interfaceType.GetGenericArguments().Single())
            .SingleOrDefault();

        if (responseType is not null)
        {
            return responseType;
        }

        var isEmptyResponse = mediatrType
            .GetInterfaces()
            .SingleOrDefault(
                interfaceType => interfaceType == typeof(IRequest) || interfaceType == typeof(INotification));

        if (isEmptyResponse is not null)
        {
            return typeof(Unit);
        }

        return null;
    }
}