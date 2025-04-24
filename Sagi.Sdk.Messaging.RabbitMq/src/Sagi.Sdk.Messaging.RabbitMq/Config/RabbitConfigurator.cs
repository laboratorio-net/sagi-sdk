using Microsoft.Extensions.DependencyInjection;

using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq.Config;

public class RabbitConfigurator : IDisposable
{
    public RabbitConfigurator(
        IServiceCollection services,
        CancellationToken token)
    {
        Token = token;
        Services = services;
    }

    private CancellationToken Token { get; }
    private IServiceCollection Services { get; }
    private IConnection? Connection { get; set; }
    private RabbitOptions? Options { get; set; }
    private ChannelConfigurator? ChannelConfig { get; set; }

    public void ConfigureRabbit(Action<RabbitOptions> configure)
    {
        Options = new();
        configure(Options);
        Connection = ConfigureFactory();
        ChannelConfig = new(Connection, Token);
    }

    public void ConfigureEndpoint<TMessage>(
        Action<EndpointConfigurator<TMessage>> configure)
        where TMessage : class
    {
        var endpoint = new EndpointConfigurator<TMessage>(Services, Token);
        configure(endpoint);

        ChannelConfig?.DeclareExchange(endpoint.ExchangeName); //sagi.sdk.messaging.rabbitmq.consoleapp.order
        ChannelConfig?.DeclareExchange(endpoint.QueueName); //order.test
        ChannelConfig?.ExchangesBind(endpoint.ExchangeName, endpoint.QueueName);

        ChannelConfig?.DeclareQueue(endpoint);
        ChannelConfig?.QueueBind(endpoint.QueueName);

        ChannelConfig?.ConfigureBasicQos(endpoint.PrefetchCount);
        endpoint.ConfigureProducer();
    }

    private IConnection ConfigureFactory()
    {
        var factory = new ConnectionFactory()
        {
            HostName = Options?.HostName ?? "localhost",
            VirtualHost = Options?.VirtualHost ?? ConnectionFactory.DefaultVHost,
            UserName = Options?.UserName ?? ConnectionFactory.DefaultUser,
            Password = Options?.Password ?? ConnectionFactory.DefaultPass,
            Port = Options != null ? Options.Port : 5672,
        };

        var connection = factory.CreateConnectionAsync(Token).Result;

        Services.AddSingleton<IConnectionFactory>(factory);
        Services.AddSingleton<IConnection>(connection);
        Services.AddSingleton<IChannel>(connection
            .CreateChannelAsync(cancellationToken: Token).Result);

        return connection;
    }

    public void Dispose()
    {
        ChannelConfig?.Dispose();
        Connection?.Dispose();
    }
}