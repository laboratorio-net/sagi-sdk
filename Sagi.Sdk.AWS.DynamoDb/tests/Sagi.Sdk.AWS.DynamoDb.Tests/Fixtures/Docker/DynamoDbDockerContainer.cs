using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Docker;

public class DynamoDbDockerContainer : IDisposable
{
    public DynamoDbDockerContainer() => Run();

    private static IContainerService? Container { get; set; }

    public static void Run()
    {
        Container ??= new Builder()
            .UseContainer()
            .UseImage("amazon/dynamodb-local:2.6.1")
            .ExposePort(8000, 8000)
            .Command("-jar DynamoDBLocal.jar -sharedDb -inMemory")
            .WithEnvironment("AWS_ACCESS_KEY_ID", "root")
            .WithEnvironment("AWS_SECRET_ACCESS_KEY", "secret")
            .Build()
            .Start();
    }

    public void Dispose() => Container?.Dispose();
}