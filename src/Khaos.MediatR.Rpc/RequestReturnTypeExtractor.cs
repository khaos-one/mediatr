namespace Khaos.MediatR.Rpc;

public static class RequestReturnTypeExtractor
{
    public static Type? TryGetReturnType(Type mediatrType)
    {
        var typedInterface = mediatrType.GetInterface("IRequest`1");

        if (typedInterface is not null)
        {
            return typedInterface.GetGenericArguments().FirstOrDefault();
        }

        return null;
    }
}