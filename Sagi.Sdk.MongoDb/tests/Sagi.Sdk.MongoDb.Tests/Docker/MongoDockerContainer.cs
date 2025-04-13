using Ductus.FluentDocker.Builders;
using Ductus.FluentDocker.Services;

namespace Sagi.Sdk.MongoDb.Tests.Docker;

public class MongoDockerContainer : IDisposable
{
    public MongoDockerContainer() => Run();

    private static IContainerService? Container { get; set; }

    public static void Run()
    {
        Container ??= new Builder()
            .UseContainer()
            .UseImage("mongo")
            .ExposePort(27017, 27017)
            .Build()
            .Start();
    }

    public void Dispose() => Container?.Dispose();
}