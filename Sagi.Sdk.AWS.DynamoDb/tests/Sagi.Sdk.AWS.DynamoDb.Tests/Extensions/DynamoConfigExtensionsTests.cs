using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Extensions;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Extensions;

public class DynamoConfigExtensionsTests
{
    [Theory, AutoNSubstituteData]
    public void AddDynamoDb_WithAction_ShouldRegisterServices(
        DynamoDbConfigurator configurator)
    {
        var services = new ServiceCollection();
        void ConfiguratorMock(DynamoDbConfigurator x) => x = configurator; ;

        services.AddDynamoDb(ConfiguratorMock);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<DynamoDbConfigurator>());
        Assert.NotNull(provider.GetService<IAmazonDynamoDB>());
        Assert.NotNull(provider.GetService<IDynamoDBContext>());
        Assert.NotNull(provider.GetService(typeof(IDynamoDbContext<>)));
    }

    [Fact]
    public void AddDynamoDb_WithAction_ShouldRegisterInitializer_WhenInitializeDbIsTrue()
    {
        var services = new ServiceCollection();
        var configuratorMock = new Action<DynamoDbConfigurator>(config =>
        {
            config.InitializeDb = true;
        });

        services.AddDynamoDb(configuratorMock);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IDynamoDbTableInitializer>());
        Assert.NotNull(provider.GetService<DatabaseContextInitializer>());
    }

    [Fact]
    public void AddDynamoDb_WithoutAction_ShouldRegisterServices()
    {
        var services = new ServiceCollection();

        services.AddDynamoDb();

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IAmazonDynamoDB>());
        Assert.NotNull(provider.GetService<IDynamoDBContext>());
    }
}