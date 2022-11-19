using System.Reflection;

using Khaos.MediatR.Rpc.Codecs;

using RabbitMQ.Client;

namespace Khaos.MediatR.Rpc.RabbitMq.Publishing
{
    public class BusPublisher<T> : IBusPublisher<T>
    {
        private const string PredefinedChannelName = "publishing";

        private readonly Lazy<IRabbitMqChannel> _channel;
        private readonly IStreamCodec _streamCodec;

        private readonly Lazy<IBasicProperties> _messageProperties;
        
        public string ExchangeName { get; }

        public BusPublisher(
            string exchangeName,
            IRabbitMqChannelFactory channelFactory,
            IStreamCodec streamCodec,
            string? channelName = null)
        {
            ExchangeName = exchangeName;
            _streamCodec = streamCodec;
            _channel = new Lazy<IRabbitMqChannel>(
                () => channelFactory.GetChannel(channelName ?? PredefinedChannelName),
                LazyThreadSafetyMode.ExecutionAndPublication);

            _messageProperties = new Lazy<IBasicProperties>(
                () =>
                {
                    var props = _channel.Value.Model.CreateBasicProperties();
                    props.AppId = Assembly.GetEntryAssembly()?.GetName().Name ?? string.Empty;
                    props.ContentType = _streamCodec.OutputContentType;
                    props.Persistent = true;
                    props.DeliveryMode = 2;

                    return props;
                });
        }

        public Task Publish(T message, CancellationToken cancellationToken = default) =>
            Publish(message, string.Empty, cancellationToken);

        public Task Publish(IEnumerable<T> messages, CancellationToken cancellationToken = default) =>
            Publish(messages, string.Empty, cancellationToken);

        public Task Publish(T message, string? routingKey = null, CancellationToken cancellationToken = default) =>
            Publish(new[] { message }, routingKey, cancellationToken);

        public async Task Publish(
            IEnumerable<T> messages,
            string? routingKey = null,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            foreach (var message in messages)
            {
                using var outputStream = new MemoryStream();
                await _streamCodec.Encode(message, outputStream, cancellationToken);

                _channel.Value.Model.BasicPublish(
                    ExchangeName,
                    routingKey ?? string.Empty,
                    true,
                    _messageProperties.Value,
                    outputStream.ToArray());
            }
            
            _channel.Value.Model.WaitForConfirmsOrDie();
        }
    }
}