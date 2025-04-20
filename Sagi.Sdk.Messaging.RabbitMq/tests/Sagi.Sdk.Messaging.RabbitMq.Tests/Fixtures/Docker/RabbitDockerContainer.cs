using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;
using Ductus.FluentDocker.Services.Extensions;

namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures.Docker;

public class RabbitDockerContainer
{
    public RabbitDockerContainer() => Run();

    private static IContainerService? Container { get; set; }

    public static void Run()
    {
        Container ??= new Builder()
            .UseContainer()
            .UseImage("rabbitmq:3-management")
            .ExposePort(15672, 15672)
            .ExposePort(5672, 5672)
            .Build()
            .WaitForPort("5672/tcp", 30_000)
            .WaitForHealthy(60_000)
            .Start();
    }

    public void Dispose()
    {
        Container?.Dispose();
    }
}