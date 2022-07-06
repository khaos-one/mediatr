namespace Khaos.MediatrRpc;

public static class TypeRouteNameFactory
{
    public static string Get(Type type) => $"/$mediatr/{type.FullName}";
}