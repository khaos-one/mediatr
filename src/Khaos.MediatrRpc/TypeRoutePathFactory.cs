namespace Khaos.MediatrRpc;

public static class TypeRoutePathFactory
{
    public static string Get(Type type) => $"/$mediatr/{type.FullName}";
}