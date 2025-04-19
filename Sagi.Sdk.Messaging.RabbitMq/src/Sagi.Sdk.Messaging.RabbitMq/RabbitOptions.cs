namespace Sagi.Sdk.Messaging.RabbitMq;

public class RabbitOptions
{
    public string? HostName { get; set; }
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public string? VirtualHost { get; set; }
    public int Port { get; set; } = 5672;
}