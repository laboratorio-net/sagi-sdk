namespace Sagi.Sdk.Messaging.RabbitMq;

public class Message<TMessage> where TMessage : class
{
    public Message()
    {
        Headers = new Dictionary<string, object>();
        Arguments = new Dictionary<string, object>();
        Timestamp = DateTime.UtcNow;
        AutoAck = true;
    }

    public TMessage? Body { get; set; }
    public IDictionary<string, object> Headers { get; set; }
    public string QueueName { get; set; } = string.Empty;
    public bool Durable { get; set; } = true;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public IDictionary<string, object> Arguments { get; set; }
    public CancellationToken CancellationToken { get; internal set; }

    public string RoutingKey { get; set; } = string.Empty;
    public string ExchangeName { get; internal set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? CorrelationId { get; set; }
    public string? MessageId { get; set; }
    public bool AutoAck { get; set; }
    public bool Mandatory { get; set; }
}