using RabbitMQ.Client;

namespace Khaos.MediatR.Rpc.RabbitMq;

public class RabbitMqChannel : IRabbitMqChannel
{
    public string Name { get; }
    public IConnection Connection { get; }
    public IModel Model { get; }

    private readonly ThreadSafeBool _disposed = new();
        
    public RabbitMqChannel(string name, IConnectionFactory connectionFactory)
    {
        Name = name;
        Connection = connectionFactory.CreateConnection();
        Model = Connection.CreateModel();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            throw new InvalidOperationException("Cannot dispose already disposed RabbitMQ channel.");
        }

        _disposed.Set();
            
        Model.Dispose();
        Connection.Dispose();
    }
}