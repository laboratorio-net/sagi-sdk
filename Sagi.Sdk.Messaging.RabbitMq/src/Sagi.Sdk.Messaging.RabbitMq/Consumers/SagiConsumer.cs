using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Sagi.Sdk.Messaging.RabbitMq.Consumers;

public class SagiConsumer<TBody> :
    ISagiConsumer<TBody> where TBody : class
{
    public SagiConsumer(
        Message<TBody> message,
        IChannel channel,
        IConsumer<TBody> consumer)
    {
        Message = message ?? throw new ArgumentNullException(nameof(message));
        Channel = channel ?? throw new ArgumentNullException(nameof(channel));
        Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
    }

    private Message<TBody> Message { get; }
    private IChannel Channel { get; }
    private IConsumer<TBody> Consumer { get; }

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
            Message.LoadBody(ea.Body);
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