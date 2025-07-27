using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.MongoDb.Extensions;

namespace Sagi.Sdk.MongoDb.Tests.Extensions;

public class ServicesExtensionsTests
{
    private readonly IServiceCollection _services;

    public ServicesExtensionsTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddMongoWithAction_ShouldThrowArgumentNullException_WhenConnectionStringIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _services.AddMongo(opt =>
        {
            opt.ConnectionString = null;
            opt.DatabaseName = "TestDatabase";
        }));
    }

    [Fact]
    public void AddMongoWithAction_ShouldThrowArgumentNullException_WhenDatabaseNameIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => _services.AddMongo(opt =>
        {
            opt.ConnectionString = "mongo://localhost:27017";
            opt.DatabaseName = null;
        }));
    }
}