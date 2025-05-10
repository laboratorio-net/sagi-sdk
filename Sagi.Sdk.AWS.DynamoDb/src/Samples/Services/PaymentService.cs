using Microsoft.Extensions.Hosting;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Initializers;

using Samples.Tables;

namespace Samples.Services;

public class PaymentService(IDynamoDbContext<Payment> context) : BackgroundService
{
    private readonly IDynamoDbContext<Payment> _context = context;
    private CancellationToken _cancellationToken;

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _cancellationToken = stoppingToken;
        TablesInitializer.TableIsReady += OnTableReady;
        return Task.CompletedTask;
    }

    Task OnTableReady(object sender, DynamoDbTableEventsArgs e)
    {
        Console.WriteLine($"[Payment Service] - Table {e.TableName} is ready.");
        if (e.TableName == PaymentTable.TABLE_NAME)
        {
            Console.WriteLine("[Payment Service] - Starting processing");
            OrderService.OrderWasCreated += OnCreatedOrder;
        }

        return Task.CompletedTask;
    }

    async Task OnCreatedOrder(object sender, OrderEventArgs args)
    {
        if (!_cancellationToken.IsCancellationRequested)
        {
            var order = args.Order;
            var payment = Payment.Create(order.Id, order.Amount);

            await _context.SaveAsync(payment, PaymentTable.TABLE_NAME, _cancellationToken);
            Console.WriteLine("[Payment Service] - {0}  - Payment saved", DateTime.Now);
        }
    }
}