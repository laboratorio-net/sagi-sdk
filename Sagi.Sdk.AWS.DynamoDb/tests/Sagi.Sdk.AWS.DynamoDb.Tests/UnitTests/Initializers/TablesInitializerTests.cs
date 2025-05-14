using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using NSubstitute;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Initializers;

public class TablesInitializerTests
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly TablesInitializer _sut;

    public TablesInitializerTests()
    {
        _dynamoDb = Substitute.For<IAmazonDynamoDB>();
        _sut = new(_dynamoDb);
    }

    [Fact]
    public async Task ShouldCreateTable_WhenDoesNotExists()
    {
        var request = new CreateTableRequest();
        _dynamoDb.ListTablesAsync()
            .Returns(new ListTablesResponse());

        _sut.AddTable([request]);
        await _sut.ConfigureAsync();

        await _dynamoDb.Received().ListTablesAsync();
        await _dynamoDb.Received().CreateTableAsync(request);
    }

    [Fact]
    public async Task ShouldNoCreateTable_WhenDoesExists()
    {
        var request = new CreateTableRequest { TableName = "foo" };
        _dynamoDb.ListTablesAsync()
            .Returns(new ListTablesResponse { TableNames = [request.TableName] });

        _sut.AddTable([request]);
        await _sut.ConfigureAsync();

        await _dynamoDb.Received().ListTablesAsync();
        await _dynamoDb.DidNotReceive().CreateTableAsync(request);
    }

    [Fact]
    public async Task ShouldNoCreateTable_WhenTableListIsEmpty()
    {
        _sut.AddTable([]);
        await _sut.ConfigureAsync();

        await _dynamoDb.DidNotReceive().ListTablesAsync();
        await _dynamoDb.DidNotReceive().CreateTableAsync(Arg.Any<CreateTableRequest>());
    }

    [Theory, AutoNSubstituteData]
    public async Task ShouldEmitEvent_WhenTableIsReady(CreateTableRequest request)
    {
        _dynamoDb.ListTablesAsync()
            .Returns(new ListTablesResponse());

        bool eventEmitted = false;
        TablesInitializer.TableIsReady += async (sender, args)
            => await Task.Run(() => eventEmitted = true);

        _sut.AddTable([request]);
        await _sut.ConfigureAsync();

        Assert.True(eventEmitted, "The TableIsReady event was not emitted.");
    }
}