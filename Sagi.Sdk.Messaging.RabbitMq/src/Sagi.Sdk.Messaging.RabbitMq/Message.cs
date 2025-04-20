using System.Reflection;
using System.Text;
using System.Text.Json;

using RabbitMQ.Client;

namespace Sagi.Sdk.Messaging.RabbitMq;

public class Message<TBody> where TBody : class
{
    public Message()
    {
        Headers = new Dictionary<string, object>();
        Timestamp = DateTime.UtcNow;
        AutoAck = true;
        QueueName = string.Empty;
        RoutingKey = string.Empty;
        ExchangeName = string.Empty;
        Mandatory = false;
        AppId = string.Empty;
        UserId = string.Empty;
    }

    public TBody? Body { get; set; }
    public IDictionary<string, object> Headers { get; set; }
    public string QueueName { get; set; }
    public CancellationToken CancellationToken { get; internal set; }
    public string RoutingKey { get; set; }
    public string ExchangeName { get; internal set; }
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public string? MessageId { get; set; }
    public bool AutoAck { get; set; }
    public bool Mandatory { get; set; }
    public string AppId { get; set; }
    public string UserId { get; set; }

    public byte[] GetBodyBytes(TBody body)
    {
        ArgumentNullException.ThrowIfNull(body);
        var json = JsonSerializer.Serialize(body);
        return Encoding.UTF8.GetBytes(json);
    }

    public void LoadBody(ReadOnlyMemory<byte> body)
    {
        var array = body.ToArray();
        var message = Encoding.UTF8.GetString(array);
        Body = JsonSerializer.Deserialize<TBody>(message)!;
    }

    public void LoadProperties(BasicProperties prop)
    {
        MessageId = prop.MessageId;
        Timestamp = DateTimeOffset.FromUnixTimeSeconds(prop.Timestamp.UnixTime).UtcDateTime;
        CorrelationId = prop.CorrelationId;
        AppId = prop.AppId ?? "";
        UserId = prop.UserId ?? "";

        foreach (var header in prop.Headers!)
        {
            Headers.Add(header.Key, header.Value!);
        }
    }

    public BasicProperties CreateProperties(string user,
        IEnumerable<KeyValuePair<string, object>> headers)
    {
        ArgumentNullException.ThrowIfNull(headers);

        return new()
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent,
            MessageId = Guid.NewGuid().ToString(),
            Timestamp = new(DateTimeOffset.UtcNow.ToUnixTimeSeconds()),
            Type = Body?.GetType().Name.ToLower(),
            AppId = Assembly.GetEntryAssembly()?.GetName().Name ?? "unknown-app",
            UserId = user,
            Headers = new Dictionary<string, object?>(headers!)
            {
                //TODO: change to open telemetry
                {"x-trace-id", $"{Guid.NewGuid()}"}
            }
        };
    }
}