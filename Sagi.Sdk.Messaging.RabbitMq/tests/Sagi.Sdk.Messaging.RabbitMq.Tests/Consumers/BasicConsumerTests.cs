using System.Text.Json;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Consumers;
using Sagi.Sdk.Messaging.RabbitMq.Extensions;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Consumers;

public class FakeMessage
{
    public string Foo { get; set; } = "Bar";
}

public class FakeConsumer : IConsumer<FakeMessage>
{
    public Task ConsumeAsync(Message<FakeMessage> message)
    {
        Console.WriteLine(JsonSerializer.Serialize(message.Body));

        return Task.CompletedTask;
    }
}

public class IntegrationTestsFixture
{
    public IntegrationTestsFixture()
    {
        var services = new ServiceCollection();
        services.AddRabbit(o =>
        {
            o.HostName = "localhost";
            o.UserName = "guest";
            o.Password = "guest";
            o.VirtualHost = "/";

        }, config => config.ConfigureEndpoint<FakeMessage>(endpoint =>
        {
            endpoint.QueueName = "fake.messages.tests.new";
            endpoint.ConfigureConsumer<FakeConsumer>(c =>
            {
                c.Durable = true;
                c.AutoAck = true;
            });
        }));

        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; private set; }
}

public class BasicConsumerTests : IClassFixture<IntegrationTestsFixture>
{
    private readonly ISagiConsumer<FakeMessage> _sut;

    public BasicConsumerTests(IntegrationTestsFixture fixture)
    {
        _sut = fixture.ServiceProvider
            .GetRequiredService<ISagiConsumer<FakeMessage>>();
    }

    [Fact]
    public async Task Test1()
    {
        await _sut.ConsumeAsync();
        Assert.True(true);
    }
}
