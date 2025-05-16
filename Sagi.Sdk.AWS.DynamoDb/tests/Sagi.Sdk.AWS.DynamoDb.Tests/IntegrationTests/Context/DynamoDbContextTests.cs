using Amazon.DynamoDBv2.DocumentModel;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
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
        await _sut.SaveAsync(model, InsertTestTable.TABLE_NAME, _cancellationToken);
        var filter = new QueryFilter();
        filter.AddCondition(nameof(FakeModel.Id), QueryOperator.Equal, model.Id);
        filter.AddCondition(nameof(FakeModel.CreatedAt), QueryOperator.Equal, model.CreatedAt);

        var result = await _sut.GetSingleAsync(filter, InsertTestTable.TABLE_NAME, _cancellationToken);

        Assert.NotNull(result);
        Assert.IsType<FakeModel>(result);
        // Assert.Equivalent(model, result);
    }
}