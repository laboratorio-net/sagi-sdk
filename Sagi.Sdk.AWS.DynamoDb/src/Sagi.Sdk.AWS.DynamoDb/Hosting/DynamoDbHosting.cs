using Microsoft.Extensions.Hosting;

using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Extensions;

namespace Sagi.Sdk.AWS.DynamoDb.Hosting;

public class DynamoDbHosting
{
    public static IServiceProvider? Provider { get; private set; }
    private static IHost? DynamoDbHost { get; set; }

    public static async Task RunAsync(Action<DynamoDbConfigurator> action)
    {
        DynamoDbHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services => 
                services.AddDynamoDb(action))
            .Build();

        Provider = DynamoDbHost.Services;
        await DynamoDbHost.RunAsync();
    }

    public static async Task StopAsync()
    {
        await Task.Run(async () =>
       {
           await Task.Delay(5000);
           Console.WriteLine("Stopping host...");
           await DynamoDbHost!.StopAsync();
           DynamoDbHost!.Dispose();
       });
    }
}