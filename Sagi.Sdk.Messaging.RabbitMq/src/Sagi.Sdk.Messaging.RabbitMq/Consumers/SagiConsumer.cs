using System.Text;
using System.Text.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sagi.Sdk.Messaging.RabbitMq.Consumers;

public class SagiConsumer<TMessage> :
    ISagiConsumer<TMessage> where TMessage : class
{
    public SagiConsumer(
        Message<TMessage> message,
        IChannel channel,
        IConsumer<TMessage> consumer)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }

    private Message<TMessage> Message { get; }
    private IChannel Channel { get; }
    private IConsumer<TMessage> Consumer { get; }

    public async Task ConsumeAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(Channel);
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

            Message.Body = JsonSerializer.Deserialize<TMessage>(message)!;
            await Consumer.ConsumeAsync(Message);

            if (Message.AutoAck == true)
            {
                await Channel.BasicAckAsync(
                    ea.DeliveryTag,
                    multiple: false,
                    Message.CancellationToken);
            }
        };
    }

    private Task ConsumeAsync(AsyncEventingBasicConsumer consumer)
    {
        return Channel.BasicConsumeAsync(
            queue: Message.QueueName!,
            autoAck: Message.AutoAck,
            consumer: consumer,
            cancellationToken: Message.CancellationToken
        );
    }
}