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
    });

Console.CancelKeyPress += (obj, args) =>
{
    DynamoDbHosting.StopAsync().Wait();
};