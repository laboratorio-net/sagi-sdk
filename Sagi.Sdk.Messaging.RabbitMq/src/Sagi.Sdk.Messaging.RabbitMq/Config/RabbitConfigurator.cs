using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq.Config;

public class RabbitConfigurator
{
    public RabbitConfigurator(
        IConnection connection,
        IServiceCollection services,
        CancellationToken token)
    {
        Token = token;
        Services = services;
        Channel = connection
            .CreateChannelAsync(cancellationToken: token).Result; ;
    }

    private IChannel Channel { get; }
    private CancellationToken Token { get; }
    private IServiceCollection Services { get; }

    public void ConfigureEndpoint<TMessage>(
        Action<EndpointConfigurator<TMessage>> action)
        where TMessage : class
    {
        var endpoint = new EndpointConfigurator<TMessage>(Services, Token);
        action.Invoke(endpoint);

        DeclareChannel(endpoint);
        DeclareQueue(endpoint);
        QueueBind(endpoint);
        endpoint.ConfigureProducer();
    }

    private void DeclareChannel<TMessage>(
        EndpointConfigurator<TMessage> endpoint) where TMessage : class
    {
        Channel
            .ExchangeDeclareAsync(
                endpoint.ExchangeName,
                ExchangeType.Fanout,
                cancellationToken: Token)
            .Wait();
    }

    private void DeclareQueue<TMessage>(
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

    private void QueueBind<TMessage>(
        EndpointConfigurator<TMessage> endpoint) where TMessage : class
    {
        Channel
            .QueueBindAsync(
                endpoint.QueueName,
                endpoint.ExchangeName,
                endpoint.RoutingKey, null,
                cancellationToken: Token)
            .Wait();
    }

}