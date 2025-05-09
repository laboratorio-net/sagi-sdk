using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Context;
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

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? Status { get; set; }

    private static string SortStatus()
    {
        var statusList = new string[] { "Pending", "Processing", "Done" };
        return statusList[Random.Shared.Next(0, statusList.Length - 1)];
    }

    public static Order Create() => new()
    {
        Id = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        Status = SortStatus(),
    };
}

public class OrderService(IDynamoDbContext<Order> context) : BackgroundService
{
    private readonly IDynamoDbContext<Order> _context = context;
    private readonly DynamoDBOperationConfig _config = new()
    {
        OverrideTableName = FirstTable.TABLE_NAME
    };

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var counter = 0;
        var delayInSeconds = 5;

        await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await _context.SaveAsync(Order.Create(), FirstTable.TABLE_NAME, stoppingToken);

            counter++;
            Console.WriteLine("{0} | [{1}] - Record saved", DateTime.Now, counter);
            await Task.Delay(TimeSpan.FromSeconds(delayInSeconds), stoppingToken);
        }
    }
}