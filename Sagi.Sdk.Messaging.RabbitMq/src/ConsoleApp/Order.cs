using System.Text.Json;

using Microsoft.Extensions.Hosting;

using Sagi.Sdk.Messaging.RabbitMq.Consumers;
using Sagi.Sdk.Messaging.RabbitMq.Producers;

namespace Sagi.Sdk.Messaging.RabbitMq.ConsoleApp;

public record Order(Guid Id, decimal Price);


public class OrderConsumer : IConsumer<Order>
{
    public async Task ConsumeAsync(Message<Order> message)
    {
        Console.WriteLine("Order: " + JsonSerializer.Serialize(message.Body));
        await Task.Delay(3000);
    }
}

public class StockConsumer : IConsumer<Order>
{
    public async Task ConsumeAsync(Message<Order> message)
    {
        Console.WriteLine("Stock: " + JsonSerializer.Serialize(message.Body));
        await Task.Delay(3000);
    }
}

public class ProducerService : BackgroundService
{
    private readonly IProducer<Order> _producer;

    public ProducerService(IProducer<Order> producer)
    {
        _producer = producer ?? throw new ArgumentNullException(nameof(producer));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await _producer.PublishAsync(new Order(Guid.NewGuid(), 5), stoppingToken);
            await Task.Delay(100, stoppingToken);
        }
    }
}