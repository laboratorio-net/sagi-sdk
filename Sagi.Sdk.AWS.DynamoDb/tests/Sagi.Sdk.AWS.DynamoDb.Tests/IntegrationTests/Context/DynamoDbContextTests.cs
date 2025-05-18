using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using AutoFixture;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Pages;
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
    public async Task GetSingleAsync_ShouldRecoveryRecord_WhenDoesExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        await _sut.SaveAsync(model, GetSingleTestTable.TABLE_NAME, _cancellationToken);

        //act
        var result = await _sut.GetSingleAsync(filter,
            GetSingleTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);
        Assert.Equal(model.Id, result.Id);
    }

    [Theory, AutoNSubstituteData]
    public async Task GetSingleAsync_ShouldNotRecoveryRecord_WhenDoesNotExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        //act
        var result = await _sut.GetSingleAsync(filter,
            GetSingleTestTable.TABLE_NAME, _cancellationToken);

        Assert.Null(result);
    }

    [Theory, AutoNSubstituteData]
    public async Task GetAll_ShouldGetPaginatedRecords(IFixture fixture)
    {
        var totalRecords = 15;
        var pageSize = 5;

        var records = fixture.CreateMany<FakeModel>(totalRecords).ToList();
        var tasks = new List<Task>();

        records.ForEach(model => _sut.SaveAsync(model,
            GetAllTestTable.TABLE_NAME, _cancellationToken));

        await Task.WhenAll(tasks);

        PageResult<FakeModel> result = new();

        do
        {
            var query = new PageQuery()
            {
                PageSize = pageSize,
                PageToken = result.PageToken,
            };

            result = await _sut.GetAll(query,
                GetAllTestTable.TABLE_NAME, _cancellationToken);

            Assert.NotNull(result);
            Assert.IsType<PageResult<FakeModel>>(result);

            result.Items.ToList().ForEach(item =>
                Assert.Contains(records, r => r.Id == item.Id));

        } while (result.HasNextPage);
    }

    [Theory, AutoNSubstituteData]
    public async Task SaveAsync_ShouldInsertRecordInDynamoDb(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        //act
        await _sut.SaveAsync(model, InsertTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);

        Assert.Equal(model.Id, result.Id);
        Assert.Equal(model.Name, result.Name);
        Asserts.Equal(model.CreatedAt, result.CreatedAt);
    }

    [Theory, AutoNSubstituteData]
    public async Task SaveAsync_ShouldUpdateARecord_WhenAlreadyExists(
        FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);
        var newName = "foo";

        await _sut.SaveAsync(model, InsertTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(model.Name, result.Name);

        //act
        await _sut.SaveAsync(model.WithName(newName), InsertTestTable.TABLE_NAME, _cancellationToken);
        result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(newName, result.Name);
    }

    [Theory, AutoNSubstituteData]
    public async Task DeleteAsync_ShouldRemoveRecord_WhenDoesExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        await _sut.SaveAsync(model, DeleteTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, DeleteTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);
        Assert.Equal(model.Id, result.Id);

        var conditions = new Dictionary<string, AttributeValue>() {
            { nameof(FakeModel.Id), new AttributeValue { S = model.Id } }
        };

        //act
        await _sut.DeleteAsync(conditions, DeleteTestTable.TABLE_NAME, _cancellationToken);

        result = await _sut.GetSingleAsync(filter, DeleteTestTable.TABLE_NAME, _cancellationToken);
        Assert.Null(result);
    }

    [Theory, AutoNSubstituteData]
    public async Task DeleteAsync_ShouldNotRemoveRecord_WhenDoesNotExists(FakeModel model)
    {
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);

        await _sut.SaveAsync(model, DeleteTestTable.TABLE_NAME, _cancellationToken);
        var result = await _sut.GetSingleAsync(filter, DeleteTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(model.Id, result.Id);

        var conditions = new Dictionary<string, AttributeValue>() {
            { nameof(FakeModel.Id), new AttributeValue { S = Guid.NewGuid().ToString() } }
        };

        //act
        await _sut.DeleteAsync(conditions, DeleteTestTable.TABLE_NAME, _cancellationToken);

        result = await _sut.GetSingleAsync(filter, DeleteTestTable.TABLE_NAME, _cancellationToken);
        Assert.NotNull(result);
        Assert.Equal(model.Id, result.Id);
    }
}