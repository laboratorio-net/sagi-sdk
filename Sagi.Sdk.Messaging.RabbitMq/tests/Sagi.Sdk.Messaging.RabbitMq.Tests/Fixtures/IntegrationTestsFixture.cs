using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Extensions;
using Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures.Fakers;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures;

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
            endpoint.ConfigureConsumer<FakeConsumer>(c => c.AutoAck = true);
        }));

        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; private set; }
}