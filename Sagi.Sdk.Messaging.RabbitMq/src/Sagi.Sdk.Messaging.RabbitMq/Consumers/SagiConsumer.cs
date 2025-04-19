using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sagi.Sdk.Messaging.RabbitMq.Consumers;

public class SagiConsumer<TMessage> : 
    ISagiConsumer<TMessage> where TMessage : class
{
    private readonly Message<TMessage> _message;
    private readonly IChannel _channel;
    private readonly IConsumer<TMessage> _consumer;

    public SagiConsumer(
        Message<TMessage> message,
        IChannel channel,
        IConsumer<TMessage> consumer)
    {
        _message = message ?? throw new ArgumentNullException(nameof(message));
        _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }

    public async Task ConsumeAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        ConfigureReceiver(consumer);
        await ConsumeAsync(consumer);
    }

    private void ConfigureReceiver(AsyncEventingBasicConsumer consumer)
    {
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            ArgumentException.ThrowIfNullOrEmpty(message);

            _message.Body = JsonSerializer.Deserialize<TMessage>(message)!;
            await _consumer.ConsumeAsync(_message);

            if (_message.AutoAck == true)
            {
                await _channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    _message.CancellationToken);
            }
        };
    }

    private Task ConsumeAsync(AsyncEventingBasicConsumer consumer)
        => _channel.BasicConsumeAsync(
            queue: _message.QueueName!,
            autoAck: _message.AutoAck,
            consumer: consumer,
            cancellationToken: _message.CancellationToken
        );

}
