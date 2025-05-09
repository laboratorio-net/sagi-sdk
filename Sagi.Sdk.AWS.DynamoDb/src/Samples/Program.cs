using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
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
    private CancellationToken _cancellationToken;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        TablesInitializer.TableIsReady += OnTableReady;
        return Task.CompletedTask;
    }

    async Task OnTableReady(object sender, DynamoDbTableEventsArgs e)
    {
        Console.WriteLine($"Table {e.TableName} is ready.");
        if (e.TableName == FirstTable.TABLE_NAME)
        {
            Console.WriteLine("Starting processing");
            await StartProcess();
        }
    }

    async Task StartProcess()
    {
        var counter = 0;

        while (!_cancellationToken.IsCancellationRequested)
        {
            await _context.SaveAsync(Order.Create(), FirstTable.TABLE_NAME, _cancellationToken);

            counter++;
            Console.WriteLine("{0} | [{1}] - Record saved", DateTime.Now, counter);
            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationToken);
        }
    }
}