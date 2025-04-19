using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Consumers;
using Sagi.Sdk.Messaging.RabbitMq.Producers;

namespace Sagi.Sdk.Messaging.RabbitMq.Config;

public class EndpointConfigurator<TMessage> where TMessage : class
{
    public EndpointConfigurator(
        IServiceCollection services,
        CancellationToken cancellationToken)
    {
        Arguments = new Dictionary<string, object>();
        Token = cancellationToken;
        Services = services;
    }

    public string QueueName { get; set; } = string.Empty;
    public string RoutingKey { get; set; } = string.Empty;
    public string ExchangeName { get; set; } = typeof(TMessage)?.FullName?.ToLower()!;
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object> Arguments { get; set; }
    private CancellationToken Token { get; }
    private IServiceCollection Services { get; }

    public void ConfigureConsumer<TConsumer>(Action<Message<TMessage>> action)
        where TConsumer : class, IConsumer<TMessage>
    {
        var message = new Message<TMessage>
        {
            CancellationToken = Token,
            QueueName = QueueName,
            ExchangeName = ExchangeName,
        };
        action.Invoke(message);
        Services.AddSingleton(message);

        Services.AddSingleton<IConsumer<TMessage>, TConsumer>();
        Services.AddSingleton(typeof(ISagiConsumer<>), typeof(SagiConsumer<>));
    }

    public void ConfigureProducer()
    {
        Services.AddSingleton<IProducer<TMessage>, SagiProducer<TMessage>>();
    }
}