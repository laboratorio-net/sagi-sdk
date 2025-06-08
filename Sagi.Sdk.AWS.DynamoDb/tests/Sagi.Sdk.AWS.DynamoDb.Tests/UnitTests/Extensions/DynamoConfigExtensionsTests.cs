using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Extensions;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Extensions;

public class DynamoConfigExtensionsTests
{
    [Theory, AutoNSubstituteData]
    public void AddDynamoDb_WithAction_ShouldRegisterServices(
        DynamoDbConfigurator configurator)
    {
        var configuratorMock = new Action<DynamoDbConfigurator>(config =>
        {
            config.ServiceURL = configurator.ServiceURL;
        });

        var services = new ServiceCollection();
        services.AddDynamoDb(configuratorMock);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<DynamoDbConfigurator>());
        Assert.NotNull(provider.GetService<IAmazonDynamoDB>());
        Assert.NotNull(provider.GetService<IDynamoDBContext>());
        Assert.NotNull(provider.GetService(typeof(IDynamoDbContext<FakeModel>)));
    }

    [Theory, AutoNSubstituteData]
    public void AddDynamoDb_WithAction_ShouldRegisterInitializer_WhenInitializeDbIsTrue(
        DynamoDbConfigurator configurator
    )
    {
        var configuratorMock = new Action<DynamoDbConfigurator>(config =>
        {
            config.InitializeDb = true;
            config.ServiceURL = configurator.ServiceURL;
        });

        var services = new ServiceCollection();
        services.AddDynamoDb(configuratorMock);

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IDynamoDbTableInitializer>());
    }
}