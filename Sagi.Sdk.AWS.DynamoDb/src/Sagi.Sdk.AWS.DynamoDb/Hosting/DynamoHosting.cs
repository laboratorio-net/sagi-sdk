using Microsoft.Extensions.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Extensions;

namespace Sagi.Sdk.AWS.DynamoDb.Hosting;

public class DynamoHosting
{
    public static IServiceProvider? Provider { get; private set; }
    private static IHost? DynamoDbHost { get; set; }

    public static async Task RunAsync()
    {
        DynamoDbHost = Host
            .CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.AddDynamoDb(x =>
                {
                    x.Accesskey = "root";
                    x.SecretKey = "secret";
                    x.ServiceURL = "http://localhost:8000";
                    x.InitializeDb = true;
                });
            }).Build();

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