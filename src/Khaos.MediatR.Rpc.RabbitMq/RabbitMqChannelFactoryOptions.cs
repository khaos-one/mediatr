namespace Khaos.MediatR.Rpc.RabbitMq;

public class RabbitMqChannelFactoryOptions 
{
    public string? HostName { get; init; }
    public ushort? Port { get; init; }
    public string? UserName { get; init; }
    public string? Password { get; init; }
    public string? VirtualHost { get; init; }
}