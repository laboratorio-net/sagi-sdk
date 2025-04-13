using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.MongoDb.Context;
using Sagi.Sdk.MongoDb.Extensions;
using Sagi.Sdk.MongoDb.Tests.Fakes;

namespace Sagi.Sdk.MongoDb.Tests.Fixtures;

public class IntegrationTestsFixture
{
    public IntegrationTestsFixture()
    {
        var services = new ServiceCollection();
        services.AddMongo(o =>
        {
            o.ConnectionString = "mongodb://localhost:27017";
            o.DatabaseName = "FakeDatabase";
        });

        services.AddSingleton<MongoContext<FakeDocument>, FakeContext>();
        ServiceProvider = services.BuildServiceProvider();
    }

    public IServiceProvider ServiceProvider { get; private set; }
}