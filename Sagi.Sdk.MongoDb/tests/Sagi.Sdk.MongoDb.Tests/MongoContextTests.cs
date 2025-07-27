using Microsoft.Extensions.DependencyInjection;

using MongoDB.Driver;

using Sagi.Sdk.MongoDb.Context;
using Sagi.Sdk.MongoDb.Tests.Fixtures;
using Sagi.Sdk.MongoDb.Tests.Fixtures.Docker;
using Sagi.Sdk.MongoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.MongoDb.Tests;

public class MongoContextTests :
    BaseDockerTests,
    IClassFixture<IntegrationTestsFixture>
{
    private readonly MongoContext<FakeDocument> _sut;
    private readonly CancellationToken _cancellationToken = new();

    public MongoContextTests(IntegrationTestsFixture fixture)
    {
        _sut = fixture.ServiceProvider
            .GetRequiredService<MongoContext<FakeDocument>>();

        var database = fixture.ServiceProvider
            .GetRequiredService<IMongoDatabase>();

        database.DropCollection(_sut.CollectionName, _cancellationToken);
    }

    [Fact]
    public async Task InsertAsync_ShouldInsertRecordInMongoDb()
    {
        var document = new FakeDocument();

        await _sut.InsertAsync(document, _cancellationToken);
        var result = await _sut.GetByIdAsync(document.Id, _cancellationToken);

        Assert.NotNull(result);
        Assert.Equivalent(document, result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateRecordInMongoDb()
    {
        var document = new FakeDocument();
        await _sut.InsertAsync(document, _cancellationToken);

        document.Foo = "UpdatedFoo";
        await _sut.UpdateAsync(document, _cancellationToken);

        var updatedResult = await _sut.GetByIdAsync(document.Id, _cancellationToken);
        Assert.NotNull(updatedResult);
        Assert.Equivalent(document, updatedResult);
    }

    [Fact]
    public async Task GetAll_ShouldReturnData_WhenDoesExists()
    {
        var document1 = new FakeDocument();
        var document2 = new FakeDocument();
        var documents = new List<FakeDocument> { document1, document2 };

        await _sut.InserMany(documents, _cancellationToken);
        var result = await _sut.GetAll(x => true, _cancellationToken);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Contains(result, x => x.Id == document1.Id);
        Assert.Contains(result, x => x.Id == document2.Id);
    }

    [Fact]
    public async Task GetAll_ShouldReturnEmpytCollection_WheHasNotData()
    {
        var result = await _sut.GetAll(x => true, _cancellationToken);
        Assert.Empty(result);        
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnTrue_WhenDocumentExists()
    {
        var document = new FakeDocument();
        await _sut.InsertAsync(document, _cancellationToken);

        var result = await _sut.ExistsAsync(document.Id, _cancellationToken);
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_ShouldReturnFalse_WhenDocumentDoesNotExists()
    {
        var document = new FakeDocument();
        await _sut.InsertAsync(document, _cancellationToken);

        var result = await _sut.ExistsAsync(Guid.NewGuid().ToString(), _cancellationToken);
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteDocument()
    {
        var document = new FakeDocument();
        await _sut.InsertAsync(document, _cancellationToken);

        await _sut.DeleteAsync(document.Id, _cancellationToken);
        var result = await _sut.GetByIdAsync(document.Id, _cancellationToken);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnDocument_WhenExists()
    {
        var document = new FakeDocument();
        await _sut.InsertAsync(document, _cancellationToken);

        var result = await _sut.GetByIdAsync(document.Id, _cancellationToken);
        Assert.NotNull(result);
        Assert.Equivalent(document, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenDoesNotExists()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid().ToString(), _cancellationToken);
        Assert.Null(result);
    }
}