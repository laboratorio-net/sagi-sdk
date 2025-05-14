using Amazon.DynamoDBv2;
using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Factories;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Factories;

public class AmazonDynamoDBClientFactoryTests
{
    [Fact]
    public void Create_ShouldReturnAmazonDynamoDBClient()
    {
        var client = AmazonDynamoDBClientFactory.Create();

        Assert.NotNull(client);
        Assert.IsType<AmazonDynamoDBClient>(client);
    }

    [Theory, AutoNSubstituteData]
    public void Create_WithConfigurator_ShouldReturnAmazonDynamoDBClient(
        DynamoDbConfigurator configurator)
    {
        configurator.SessionToken = "";

        var client = AmazonDynamoDBClientFactory.Create(configurator);

        Assert.NotNull(client);
        Assert.IsType<AmazonDynamoDBClient>(client);
    }

    [Theory, AutoNSubstituteData]
    public void Create_WithConfiguratorAndSessionToken_ShouldReturnAmazonDynamoDBClient(
        DynamoDbConfigurator configurator)
    {
        var client = AmazonDynamoDBClientFactory.Create(configurator);

        Assert.NotNull(client);
        Assert.IsType<AmazonDynamoDBClient>(client);
    }
}