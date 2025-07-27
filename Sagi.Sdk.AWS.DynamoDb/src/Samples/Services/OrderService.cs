using Microsoft.Extensions.Hosting;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Initializers;

using Samples.Entities;
using Samples.Tables;

namespace Samples.Services;

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
        Console.WriteLine($"[Order Service] - Table {e.TableName} is ready.");
        if (e.TableName == OrderTable.TABLE_NAME)
        {
            Console.WriteLine("[Order Service] - Starting processing");
            await StartProcess();
        }
    }

    async Task StartProcess()
    {
        var counter = 0;

        while (!_cancellationToken.IsCancellationRequested)
        {
            var order = Order.Create();
            await _context.SaveAsync(order, OrderTable.TABLE_NAME, _cancellationToken);
            OnCreatedOrder(OrderEventArgs.Create(order));

            counter++;
            Console.WriteLine("[Order Service] - {0} | [{1}] - Order saved", DateTime.Now, counter);
            await Task.Delay(TimeSpan.FromSeconds(3), _cancellationToken);
        }
    }

    private void OnCreatedOrder(OrderEventArgs args) =>
        OrderWasCreated?.Invoke(this, args);

    public static event OrderEventHandler? OrderWasCreated;
}

public class OrderEventArgs : EventArgs
{
    public required Order Order { get; set; }
    public DateTime TimeStamp { get; set; }

    public static OrderEventArgs Create(Order order) => new()
    {
        Order = order,
        TimeStamp = DateTime.UtcNow,
    };
}

public delegate Task OrderEventHandler(object sender, OrderEventArgs e);