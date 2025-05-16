using Microsoft.Extensions.DependencyInjection;
using Sagi.Sdk.AWS.DynamoDb.Extensions;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Tables;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

public class IntegrationTestsFixture
{
    public IntegrationTestsFixture()
    {
        var services = new ServiceCollection();
        services.AddDynamoDb(x =>
        {
            x.Accesskey = "root";
            x.SecretKey = "secret";
            x.ServiceURL = "http://localhost:8000";
            x.InitializeDb = true;
            x.ConfigureTable(new InsertTestTable());
        });

        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; private set; }
}