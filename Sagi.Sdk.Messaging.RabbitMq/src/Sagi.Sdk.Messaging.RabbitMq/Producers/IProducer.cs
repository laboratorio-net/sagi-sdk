namespace Sagi.Sdk.Messaging.RabbitMq.Producers;

public interface IProducer<TMessage> where TMessage : class
{
    Task PublishAsync(TMessage message, CancellationToken cancellationToken);
}