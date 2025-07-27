using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Hosting;

using Samples.Services;
using Samples.Tables;

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

var options = config.GetSection("AWS").Get<AwsOptions>()!;

await DynamoDbHosting
    .RunAsync(x =>
    {
        x.Accesskey = options.Accesskey;
        x.SecretKey = options.SecretKey;
        x.ServiceURL = options.ServiceUrl;
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

record AwsOptions(string Accesskey, string SecretKey, string ServiceUrl);