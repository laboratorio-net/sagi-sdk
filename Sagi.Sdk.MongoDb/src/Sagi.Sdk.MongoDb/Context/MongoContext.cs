using System.Linq.Expressions;

using MongoDB.Driver;

namespace Sagi.Sdk.MongoDb.Context;

public abstract class MongoContext<T> : IMongoContext<T> where T : Document
{
    public MongoContext(IMongoDatabase database) 
        => Collection = database.GetCollection<T>(CollectionName);

    public abstract string CollectionName { get; }

    protected IMongoCollection<T> Collection { get; }

    public Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return Collection.Find(predicate).ToListAsync(cancellationToken);
    }

    public Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default) =>
        Collection.Find(x => x.Id == id).FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        return (await Collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken)) > 0;
    }

    public Task InsertAsync(T model, CancellationToken cancellationToken = default) =>
        Collection.InsertOneAsync(model, options: null, cancellationToken);

    public Task InserMany(List<T> models, CancellationToken cancellationToken = default)
    {
        InsertManyOptions options = new();
        return Collection.InsertManyAsync(models, options, cancellationToken);
    }

    public Task UpdateAsync(T model, CancellationToken cancellationToken = default)
    {
        var options = new ReplaceOptions { IsUpsert = true };
        return Collection.ReplaceOneAsync(x => x.Id == model.Id, model, options, cancellationToken);
    }

    public Task DeleteAsync(string id, CancellationToken cancellationToken = default) =>
        Collection.DeleteOneAsync(x => x.Id == id, cancellationToken);
}