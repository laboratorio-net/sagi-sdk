using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq.Config;

public class ChannelConfigurator : IDisposable
{
    public ChannelConfigurator(IConnection connection, CancellationToken token)
    {
        Token = token;
        Channel = connection
            .CreateChannelAsync(cancellationToken: token).Result;
    }

    public IChannel Channel { get; }
    private CancellationToken Token { get; }

    public void DeclareExchange(string exchangeName)
    {
        Channel
            .ExchangeDeclareAsync(exchangeName,
                ExchangeType.Fanout,
                cancellationToken: Token)
            .Wait();
    }

    public void DeclareQueue<TMessage>(
        EndpointConfigurator<TMessage> endpoint) where TMessage : class
    {
        Channel
            .QueueDeclareAsync(
                queue: endpoint.QueueName,
                durable: endpoint.Durable,
                exclusive: endpoint.Exclusive,
                autoDelete: endpoint.AutoDelete,
                arguments: endpoint.Arguments!,
                cancellationToken: Token
            ).Wait();
    }

    public void ExchangesBind(string source, string destination)
    {
        Channel
            .ExchangeBindAsync(destination, source,
                string.Empty, cancellationToken: Token)
            .Wait();
    }

    public void QueueBind(string queueName)
    {
        Channel
            .QueueBindAsync(
                queueName,
                queueName,
                string.Empty, null,
                cancellationToken: Token)
            .Wait();
    }

    public void ConfigureBasicQos(ushort prefetchCount)
    {
        Channel
            .BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: prefetchCount,
                global: false // false = aplica ao canal/consumidor atual; true = aplica ao canal inteiro
            )
            .Wait();
    }

    public void Dispose()
    {
        Channel.Dispose();
    }
}