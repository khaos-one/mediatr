namespace Khaos.MediatR.Rpc.Client;

internal static class HttpClientNameFactory
{
    public static string Get(Type assemblyMarkerType) => $"$mediatr.{assemblyMarkerType.FullName!}";
}