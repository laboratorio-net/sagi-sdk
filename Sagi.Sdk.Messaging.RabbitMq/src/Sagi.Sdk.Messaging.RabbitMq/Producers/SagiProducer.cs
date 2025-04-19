using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq.Producers;

public class SagiProducer<TMessage> :
    IProducer<TMessage> where TMessage : class
{
    public SagiProducer(
        IChannel channel,
        Message<TMessage> message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(channel);

        Channel = channel;
        Message = message;
    }

    internal IChannel Channel { get; }
    internal Message<TMessage> Message { get; }

    public async Task PublishAsync(TMessage message, CancellationToken cancellationToken)
    {
        var body = BuildMessage(message);
        var props = new BasicProperties();

        await Channel.BasicPublishAsync(
            Message.ExchangeName,
            Message.RoutingKey,
            Message.Mandatory,
            props,
            body,
            cancellationToken);
    }

    private static byte[] BuildMessage(TMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);
        var json = JsonSerializer.Serialize(message);
        return Encoding.UTF8.GetBytes(json);
    }
}