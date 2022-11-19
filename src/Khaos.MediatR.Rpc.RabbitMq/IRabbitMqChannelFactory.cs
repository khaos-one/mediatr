namespace Khaos.MediatR.Rpc.RabbitMq;

public interface IRabbitMqChannelFactory : IDisposable
{
    IRabbitMqChannel GetChannel(string name);
}