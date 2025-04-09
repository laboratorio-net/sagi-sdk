using System.Linq.Expressions;

namespace Sagi.Sdk.MongoDb.Context;

public interface IMongoContext<T> where T : class
{
    Task<List<T>> GetAll(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    Task<T> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(string id, CancellationToken cancellationToken = default);
    Task InsertAsync(T model, CancellationToken cancellationToken = default);
    Task UpdateAsync(T model, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}