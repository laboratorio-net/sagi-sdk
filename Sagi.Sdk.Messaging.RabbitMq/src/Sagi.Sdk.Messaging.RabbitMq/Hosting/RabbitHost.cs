using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sagi.Sdk.Messaging.RabbitMq.Extensions;

namespace Sagi.Sdk.Messaging.RabbitMq.Hosting;

public static class RabbitHost
{
    public static IServiceProvider? Provider { get; private set; }
    private static CancellationTokenSource Source => new();

    public static async Task RunAsync(
        Action<Config.RabbitConfigurator> configurator,
        Action<IServiceCollection> serviceConfig)
    {
        using IHost host = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddRabbit(configurator);
                serviceConfig(services);
            })
            .Build();

        Provider = host.Services;
        await host.RunAsync(Source.Token);
    }

    public static async Task StopAsync()
    {
        await Task.Run(async () =>
        {
            await Task.Delay(5000);
            Console.WriteLine("Stopping host...");
            Source.Cancel();
            Source.Dispose();
        });
    }
}