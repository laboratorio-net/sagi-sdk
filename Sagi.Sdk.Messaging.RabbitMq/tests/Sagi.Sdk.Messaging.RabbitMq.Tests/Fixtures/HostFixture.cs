using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Ductus.FluentDocker.Commands;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using NSubstitute.Extensions;


namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures;

public class HostFixture : IDisposable
{
    public HostFixture()
    {
        TestHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddHttpClient("rabbit", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(5);
                    client.BaseAddress = new Uri("http://localhost:15672");
                    client.DefaultRequestHeaders.Authorization =
                        new("Basic", new Credentials("user", "passw0rd").ToBase64());
                });

                services.AddSingleton<RabbitHttpClient>();
            })
            .Build();

        Provider = TestHost.Services;
        Task.Run(() => TestHost.Run());
    }

    private IHost TestHost { get; }

    private IServiceProvider Provider { get; }

    public TService GetService<TService>() where TService : notnull
        => Provider.GetRequiredService<TService>();

    public void Dispose()
    {
        TestHost.Dispose();
    }
}

public record QueueDto(
    string Vhost,
    string Name,
    bool Durable = true,
    bool AutoDelete = false);

public record VhostDto(string Name, string Description = "", string Tags = "");

public record GetMessageDto(
    string Vhost,
    string Name,
    string Truncate ="50000",
    string Ackmode = "ack_requeue_true",
    string Encoding = "auto",
    string Count = "1"
);

public record Credentials(string UserName, string Password)
{
    public string ToBase64()
    {
        var credentials = $"{UserName}:{Password}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials));
    }
}

public class RabbitHttpClient
{
    private readonly HttpClient _client;

    public RabbitHttpClient(IHttpClientFactory httpClientFactory)
    {
        _client = httpClientFactory.CreateClient("rabbit");
    }


    public Task CreateQueue(string vhost, string name, QueueDto body)
    {
        return _client.PutAsJsonAsync($"/api/queues/{vhost}/{name}", body);
    }

    public Task CreateVhost(string vhost, VhostDto body)
    {
        return _client.PutAsJsonAsync($"/api/vhosts/{vhost}", body);
    }

    public Task PublishMessage(string vhost, TestMessage payload)
    {
        return _client.PostAsJsonAsync($"/api/exchanges/{vhost}/amq.default/publish", payload);
    }

    public async Task<TResult> RecoveryMessage<TResult>(GetMessageDto payload)
    {
        var response = await _client.PostAsJsonAsync($"/api/queues/{payload.Vhost}/{payload.Name}/get", payload);
        var content = await response.Content.ReadAsStringAsync()!;
        var result = JsonSerializer.Deserialize<GetMessageResponse<TResult>>(content)!;
        return result.Payload;
    }
}


public class TestMessage
{
    public TestMessage(QueueDto queue, object payload)
    {
        VHost = queue.Vhost;
        RoutingKey = queue.Name;
        Payload = JsonSerializer.Serialize(payload);
    }

    [JsonPropertyName("vhost")]
    public string VHost { get; set; }

    [JsonPropertyName("routing_key")]
    public string RoutingKey { get; set; }

    [JsonPropertyName("name")]
    public string ExchangeName { get; set; } = "amq.default";

    [JsonPropertyName("properties")]
    public TestProperties Properties { get; set; } = new();

    [JsonPropertyName("delivery_mode")]
    public string DeliveryMode { get; set; } = "1";

    [JsonPropertyName("payload")]
    public string Payload { get; set; }

    [JsonPropertyName("payload_encoding")]
    public string PayloadEncoding { get; set; } = "string";

    [JsonPropertyName("headers")]
    public Dictionary<string, string> Headers { get; set; } = [];

    [JsonPropertyName("props")]
    public object Props { get; set; } = new();

    public class TestProperties
    {
        [JsonPropertyName("delivery_mode")]
        public int DeliveryMode { get; set; } = 1;
        public Dictionary<string, string> Headers { get; set; } = [];
    }
}

public class GetMessageResponse<TPayload>
{
    [JsonPropertyName("payload_bytes")]
    public int PayloadBytes { get; set; }

    [JsonPropertyName("redelivered")]
    public string Redelivered { get; set; }

    [JsonPropertyName("exchange")]
    public string Exchange { get; set; }

    [JsonPropertyName("routing_key")]
    public string RoutingKey { get; set; }

    [JsonPropertyName("message_count")]
    public int MessageCount { get; set; }

    [JsonPropertyName("properties")]
    public object Properties { get; set; }

    [JsonPropertyName("payload")]
    public TPayload Payload { get; set; }

    [JsonPropertyName("payload_encoding")]
    public string PayloadEncoding { get; set; }
}