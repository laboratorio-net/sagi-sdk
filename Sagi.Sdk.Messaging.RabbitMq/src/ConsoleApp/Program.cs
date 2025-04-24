using Sagi.Sdk.Messaging.RabbitMq.ConsoleApp;

using Microsoft.Extensions.DependencyInjection;
using Sagi.Sdk.Messaging.RabbitMq.Hosting;


await RabbitHost.RunAsync(cfg =>
{
    cfg.ConfigureRabbit(x =>
    {
        x.HostName = "localhost";
        x.Port = 5672;
        x.VirtualHost = "test3";
        x.UserName = "guest";
        x.Password = "guest";
    });

    cfg.ConfigureEndpoint<Order>(e =>
    {
        e.QueueName = "order.test.2";
        e.PrefetchCount = 2;
        e.ConfigureConsumer<OrderConsumer>(c => c.AutoAck = true);
    });

    cfg.ConfigureEndpoint<Order>(e =>
    {
        e.QueueName = "stock.test.2";
        e.PrefetchCount = 2;
        e.ConfigureConsumer<StockConsumer>(c => c.AutoAck = true);
    });

}, service => service.AddHostedService<ProducerService>());
