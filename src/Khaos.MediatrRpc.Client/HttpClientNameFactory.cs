namespace Khaos.MediatrRpc.Client;

internal static class HttpClientNameFactory
{
    public static string Get(Type assemblyMarkerType) => $"$mediatr.{assemblyMarkerType.FullName!}";
}