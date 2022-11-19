namespace Khaos.MediatR.Rpc;

public static class ConfigurationGroupNameFactory
{
    public static string Get(Type assemblyMarkerType) => $"$mediatr.{assemblyMarkerType.FullName!}";
}