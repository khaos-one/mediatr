using RabbitMQ.Client;

namespace Khaos.MediatR.Rpc.RabbitMq;

public interface IRabbitMqChannel : IDisposable
{
    string Name { get; }
    IConnection Connection { get; }
    IModel Model { get; }
}