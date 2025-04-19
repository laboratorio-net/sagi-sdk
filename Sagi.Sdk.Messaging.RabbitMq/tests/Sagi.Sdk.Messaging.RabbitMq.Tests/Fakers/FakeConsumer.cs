using System.Text.Json;

using Sagi.Sdk.Messaging.RabbitMq.Consumers;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Fakers;

public class FakeConsumer : IConsumer<FakeMessage>
{
    public Task ConsumeAsync(Message<FakeMessage> message)
    {
        Console.WriteLine(JsonSerializer.Serialize(message.Body));
        return Task.CompletedTask;
    }
}