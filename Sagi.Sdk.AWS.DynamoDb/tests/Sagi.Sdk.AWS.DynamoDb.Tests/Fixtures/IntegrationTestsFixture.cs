using Sagi.Sdk.AWS.DynamoDb.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Tables;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

public class IntegrationTestsFixture : IDisposable
{
    public IntegrationTestsFixture()
    {
        DynamoDbHosting
            .RunAsync(x =>
            {
                x.Accesskey = "root";
                x.SecretKey = "secret";
                x.ServiceURL = "http://localhost:8000";
                x.InitializeDb = true;
                x.ConfigureTable(new InsertTestTable());
            })
            .Wait();

        ServiceProvider = DynamoDbHosting.Provider!;
    }

    public IServiceProvider ServiceProvider { get; private set; }

    public void Dispose()
    {
        DynamoDbHosting
            .StopAsync()
            .Wait();
    }
}