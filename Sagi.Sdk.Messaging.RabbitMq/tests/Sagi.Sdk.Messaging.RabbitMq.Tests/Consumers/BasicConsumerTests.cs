using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.Messaging.RabbitMq.Consumers;
using Sagi.Sdk.Messaging.RabbitMq.Tests.Fakers;
using Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Consumers;

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