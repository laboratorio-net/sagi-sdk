using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Sagi.Sdk.AWS.DynamoDb.Hosting;

using Samples.Tables;

await DynamoDbHosting
    .RunAsync(x =>
    {
        x.Accesskey = "root";
        x.SecretKey = "secret";
        x.ServiceURL = "http://localhost:8000";
        x.InitializeDb = true;
        x.ConfigureTable(new FirstTable());
        x.ConfigureTable(new SecondTable());
    }, sv => sv.AddHostedService<OrderService>());

Console.CancelKeyPress += (obj, args) 
    => DynamoDbHosting.StopAsync().Wait();

public record Order(
    Guid Id,
    Guid SortKey,
    DateTime CreatedAt,
    string Status);

public class OrderService(IDynamoDBContext context) : BackgroundService
{
    private readonly IDynamoDBContext _context = context;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var counter = 0;
        var statusList = new string[] { "Pending", "Processing", "Done" };
        var config = new DynamoDBOperationConfig
        {
            OverrideTableName = FirstTable.TABLE_NAME
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var status = statusList[Random.Shared.Next(0, statusList.Length - 1)];
            var order = new Order(
                Guid.NewGuid(),
                Guid.NewGuid(),
                DateTime.Now,
                status
            );

            await _context.SaveAsync(order, config, stoppingToken);

            counter++;
            Console.WriteLine("{0} | [{1}] - Record saved", DateTime.Now, counter);
            await Task.Delay(5000, stoppingToken);
        }
    }
}