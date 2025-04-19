namespace Sagi.Sdk.Messaging.RabbitMq.Consumers;

public interface IConsumer<TMessage> where TMessage : class
{
    Task ConsumeAsync(Message<TMessage> message);
}
