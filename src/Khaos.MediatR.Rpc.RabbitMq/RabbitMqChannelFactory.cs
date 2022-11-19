using System.Collections.Concurrent;

using Microsoft.Extensions.Options;

using RabbitMQ.Client;

namespace Khaos.MediatR.Rpc.RabbitMq;

public class RabbitMqChannelFactory : IRabbitMqChannelFactory
{
    private readonly RabbitMqChannelFactoryOptions _options;
    private readonly Lazy<ConnectionFactory> _connectionFactory;
    private readonly ConcurrentDictionary<string, IRabbitMqChannel> _createdChannels = new();

    private readonly ThreadSafeBool _disposed = new();

    public RabbitMqChannelFactory(IOptions<RabbitMqChannelFactoryOptions> options)
    {
        _options = options.Value;
        _connectionFactory = new Lazy<ConnectionFactory>(
            CreateConnectionFactoryFromOptions,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public IRabbitMqChannel GetChannel(string name)
    {
        if (_disposed)
        {
            throw new InvalidOperationException("Cannot get connection on disposed RabbitMQ connection factory.");
        }
            
        var channel = _createdChannels.GetOrAdd(
            name,
            channelName => new RabbitMqChannel(channelName, _connectionFactory.Value));

        return channel;
    }

    private ConnectionFactory CreateConnectionFactoryFromOptions() =>
        new()
        {
            HostName = _options.HostName ??
                throw new ArgumentNullException(nameof(_options.HostName),
                    "RabbitMQ host name not specified."),
            Port = _options.Port ?? 5672,
            UserName = _options.UserName ??
                throw new ArgumentNullException(nameof(_options.UserName),
                    "RabbitMQ user name not specified."),
            Password = _options.Password
        };

    public void Dispose()
    {
        if (_disposed)
        {
            throw new InvalidOperationException("Cannot dispose already disposed RabbitMQ connection factory.");
        }

        _disposed.Set();

        foreach (var channel in _createdChannels)
        {
            channel.Value.Dispose();
        }
            
        _createdChannels.Clear();
    }
}