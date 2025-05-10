using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Hosting;

using Samples.Services;
using Samples.Tables;

await DynamoDbHosting
    .RunAsync(x =>
    {
        x.Accesskey = "root";
        x.SecretKey = "secret";
        x.ServiceURL = "http://localhost:8000";
        x.InitializeDb = true;
        x.ConfigureTable(new OrderTable());
        x.ConfigureTable(new PaymentTable());
    }, sv =>
    {
        sv.AddHostedService<OrderService>();
        sv.AddHostedService<PaymentService>();
    });

Console.CancelKeyPress += (obj, args)
    => DynamoDbHosting.StopAsync().Wait();
