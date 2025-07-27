using System.Text.Json.Serialization;

using Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.IntegrationTests;

public class SagiConsumerTests : IClassFixture<HostFixture>
{
    private readonly RabbitHttpClient _client;
    private readonly VhostDto _vhost;
    private readonly QueueDto _queue;


    public SagiConsumerTests(HostFixture fixture)
    {
        _client = fixture.GetService<RabbitHttpClient>();

        var timestamp = $"{DateTime.UtcNow:yyyyMMddHHmmss}";
        _vhost = new VhostDto($"testes_{timestamp}");
        _queue = new QueueDto(_vhost.Name, $"my_queue_{timestamp}");
        _client.CreateVhost(_vhost.Name, _vhost).Wait();
        _client.CreateQueue(_queue.Vhost, _queue.Name, _queue).Wait();
    }

    [Fact]
    public async Task ShouldConsumeMessageFromQueue()
    {
        try
        {
            var queue = _queue with { Name = "xpto" };
            await _client.CreateQueue(_vhost.Name, queue.Name, queue);

            var order = new OrderDto(Guid.NewGuid(), 100);
            var payload = new TestMessage(queue, order);
            await _client.PublishMessage(_vhost.Name, payload);
            var result = await _client.RecoveryMessage<OrderDto>(new GetMessageDto(_vhost.Name, queue.Name));

            Assert.Equivalent(order, result);
        }
        catch (System.Exception ex)
        {
            throw;
        }
    }
}

public class OrderDto
{
    [JsonConstructor]
    public OrderDto(Guid id, float value)
    {
        Id = id;
        Value = value;
    }

    public Guid Id { get; }
    public float Value { get; }
}