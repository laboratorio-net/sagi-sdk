using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using AutoFixture.Idioms;

using NSubstitute;

using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Pages;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Context;

public class DynamoDbContextTests
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _client;
    private readonly DynamoDbContext<FakeModel> _sut;

    public DynamoDbContextTests()
    {
        _context = Substitute.For<IDynamoDBContext>();
        _client = Substitute.For<IAmazonDynamoDB>();
        _sut = new(_context, _client);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldValidateConstructorParameters(GuardClauseAssertion assertion)
        => assertion.Verify(typeof(DynamoDbContext<FakeModel>).GetConstructors());

    [Theory, AutoNSubstituteData]
    public async Task GetAll_ShouldReturnPageResult(
        ScanResponse scanResponse, CancellationToken token)
    {
        var query = new PageQuery { PageSize = 10 };

        _client.ScanAsync(Arg.Any<ScanRequest>(), token).Returns(scanResponse);

        _context.FromDocument<FakeModel>(Arg.Any<Document>())
            .Returns(callInfo =>
            {
                var document = callInfo.Arg<Document>();
                return new FakeModel { Id = document["Id"].AsString() };
            });

        var result = await _sut.GetAll(query, FakeModel.TABLE_NAME, token);

        Assert.NotNull(result);
        Assert.Equal(scanResponse.Items.Count, result.Items.Count());
        await _client.Received().ScanAsync(Arg.Any<ScanRequest>(), token);
    }

    [Theory, AutoNSubstituteData]
    public async Task GetSingleAsync_ShouldReturnSingleItem(
        QueryFilter filter, CancellationToken token)
    {
        _context.FromQueryAsync<FakeModel>(
            Arg.Any<QueryOperationConfig>(), Arg.Any<DynamoDBOperationConfig>())
            .Returns(new FakeSearch());

        var result = await _sut.GetSingleAsync(filter, FakeModel.TABLE_NAME, token);

        Assert.NotNull(result);
        Assert.Equal("1", result.Id);

        _context.Received()
            .FromQueryAsync<FakeModel>(
            Arg.Any<QueryOperationConfig>(), Arg.Any<DynamoDBOperationConfig>());
    }

    [Theory, AutoNSubstituteData]
    public async Task SaveAsync_ShouldCallContextSaveAsync(CancellationToken token)
    {
        var model = new FakeModel { Id = "1" };
        await _sut.SaveAsync(model, FakeModel.TABLE_NAME, token);
        await _context.Received().SaveAsync(model, Arg.Any<DynamoDBOperationConfig>(), token);
    }

    [Theory, AutoNSubstituteData]
    public async Task DeleteAsync_SholdCAll_DeleteItemAsync(
        DeleteItemRequest request, CancellationToken cancellationToken)
    {
        var conditions = request.Key;

        await _sut.DeleteAsync(request.Key, FakeModel.TABLE_NAME, cancellationToken);

        await _client.Received().DeleteItemAsync(Arg.Is<DeleteItemRequest>(
            x => x.Key.Equals(conditions)), cancellationToken);
    }
}