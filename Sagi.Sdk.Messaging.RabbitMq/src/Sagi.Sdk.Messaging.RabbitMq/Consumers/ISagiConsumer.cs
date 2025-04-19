namespace Sagi.Sdk.Messaging.RabbitMq.Consumers;

public interface ISagiConsumer<TMessage> where TMessage : class
{
    Task ConsumeAsync();
}
