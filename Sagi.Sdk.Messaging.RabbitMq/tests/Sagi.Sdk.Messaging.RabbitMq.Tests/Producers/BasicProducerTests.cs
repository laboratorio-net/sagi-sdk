using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Producers;
using Sagi.Sdk.Messaging.RabbitMq.Tests.Consumers;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Producers;

public class BasicProducerTests : IClassFixture<IntegrationTestsFixture>
{
    private readonly IProducer<FakeMessage> _sut;

    public BasicProducerTests(IntegrationTestsFixture fixture)
    {
        _sut = fixture.ServiceProvider
            .GetRequiredService<IProducer<FakeMessage>>();
    }

    [Fact]
    public async Task Test1()
    {
        await _sut.PublishAsync(new FakeMessage(), CancellationToken.None);
        Assert.True(true);
    }
}
