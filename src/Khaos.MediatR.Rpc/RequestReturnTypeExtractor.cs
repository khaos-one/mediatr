namespace Khaos.MediatR.Rpc;

public static class RequestReturnTypeExtractor
{
    public static Type? TryGetReturnType(Type mediatrType)
    {
        var typedInterface = mediatrType.GetInterface("IRequest`1");
        return typedInterface?.GetGenericArguments().FirstOrDefault();
    }
}