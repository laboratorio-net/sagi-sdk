using Sagi.Sdk.AWS.DynamoDb.Hosting;

await DynamoDbHosting.RunAsync();

Console.CancelKeyPress += (obj, args )=>
{
    DynamoDbHosting.StopAsync().Wait();
};