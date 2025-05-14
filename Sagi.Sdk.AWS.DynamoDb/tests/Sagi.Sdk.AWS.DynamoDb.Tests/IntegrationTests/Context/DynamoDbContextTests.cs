using Amazon.DynamoDBv2;
using Microsoft.Extensions.DependencyInjection;
using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Docker;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.IntegrationTests.Context;

public class DynamoDbContextTests : 
    BaseDockerTests, 
    IClassFixture<IntegrationTestsFixture>
{
    private readonly DynamoDbContext<FakeModel> _sut;
    private readonly CancellationToken _cancellationToken = new();

    public DynamoDbContextTests(IntegrationTestsFixture fixture)
    {
        _sut = fixture.ServiceProvider
            .GetRequiredService<DynamoDbContext<FakeModel>>();

        var client = fixture.ServiceProvider
            .GetRequiredService<IAmazonDynamoDB>();

        client.DeleteTableAsync("FakeModel", _cancellationToken);
    }

    [Fact]
    public void SomeTest()
    {
        Assert.True(true);
    }
}
