using Amazon.DynamoDBv2.DocumentModel;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Tests.Extensions;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Docker;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Tables;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.IntegrationTests.Context;

public class DynamoDbContextTests :
    BaseDockerTests,
    IClassFixture<IntegrationTestsFixture>
{
    private readonly IDynamoDbContext<FakeModel> _sut;
    private readonly CancellationToken _cancellationToken = new();

    public DynamoDbContextTests(IntegrationTestsFixture fixture)
    {
        _sut = fixture.ServiceProvider
            .GetRequiredService<IDynamoDbContext<FakeModel>>();
    }

    [Theory, AutoNSubstituteData]
    public async Task SaveAsync_ShouldInsertRecordInDynamoDb(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        await _sut.SaveAsync(model, InsertTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);

        Assert.Equal(model.Id, result.Id);
        Assert.Equal(model.Name, result.Name);
        Asserts.Equal(model.CreatedAt, result.CreatedAt);
    }

    [Theory, AutoNSubstituteData]
    public async Task SaveAsync_ShouldUpdateARecord_WhenAlreadyExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);
        var newName = "foo";

        await _sut.SaveAsync(model, InsertTestTable.TABLE_NAME, _cancellationToken);
        await _sut.SaveAsync(model.WithName(newName), InsertTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(newName, newName);
    }

    [Theory, AutoNSubstituteData]
    public async Task GetSingleAsync_ShouldRecoveryRecord_WhenDoesExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        await _sut.SaveAsync(model, GetSingleTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, GetSingleTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);
        Assert.Equal(model.Id, result.Id);
    }

    [Theory, AutoNSubstituteData]
    public async Task GetSingleAsync_ShouldNotRecoveryRecord_WhenDoesNotExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        var result = await _sut.GetSingleAsync(filter, GetSingleTestTable.TABLE_NAME, _cancellationToken);

        Assert.Null(result);
    }
}