using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq.Producers;

public class SagiProducer<TMessage> :
    IProducer<TMessage> where TMessage : class
{
    public SagiProducer(
        IChannel channel,
        IConnectionFactory factory,
        Message<TMessage> message)
    {
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(channel);
        ArgumentNullException.ThrowIfNull(factory);

        Channel = channel;
        Message = message;
        User = factory.UserName;
    }

    internal IChannel Channel { get; }
    internal Message<TMessage> Message { get; }
    internal string User { get; }

    public Task PublishAsync(TMessage message, CancellationToken cancellationToken) 
        => PublishAsync(message, [], cancellationToken);

    public async Task PublishAsync(
        TMessage message,
        IEnumerable<KeyValuePair<string, object>> headers,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(message);
        var messageBody = Message.GetBodyBytes(message);
        var properties = Message.CreateProperties(User, headers);

        await Channel.BasicPublishAsync(
            Message?.ExchangeName!,
            Message?.RoutingKey ?? "",
            Message.Mandatory,
            properties,
            messageBody,
            cancellationToken);
    }
}