using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Extensions;

namespace Sagi.Sdk.AWS.DynamoDb.Hosting;

public class DynamoDbHosting
{
    public static IServiceProvider? Provider { get; private set; }
    private static IHost? DynamoDbHost { get; set; }

    public static async Task RunAsync(Action<DynamoDbConfigurator> configurator)
    {
        CreateHost(configurator);
        await DynamoDbHost!.RunAsync();
    }

    public static async Task RunAsync(
        Action<DynamoDbConfigurator> configurator,
        Action<IServiceCollection> servicesConfig)
    {
        CreateHost(configurator, servicesConfig);
        await DynamoDbHost!.RunAsync();
    }

    private static void CreateHost(
        Action<DynamoDbConfigurator> configurator,
        Action<IServiceCollection>? servicesConfig = null)
    {
        DynamoDbHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDynamoDb(configurator);
                servicesConfig?.Invoke(services);
            })
            .Build();

        Provider = DynamoDbHost.Services;
    }

    public static async Task StopAsync()
    {
        await Task.Run(async () =>
       {
           await Task.Delay(5000);
           await DynamoDbHost!.StopAsync();
           DynamoDbHost!.Dispose();
       });
    }
}