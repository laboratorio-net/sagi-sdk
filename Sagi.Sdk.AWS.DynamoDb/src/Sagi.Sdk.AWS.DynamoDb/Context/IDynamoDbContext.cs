using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Pages;

namespace Sagi.Sdk.AWS.DynamoDb.Context;

public interface IDynamoDbContext<TModel> where TModel : class
{
    Task<TModel?> GetSingleAsync(QueryFilter filter, string tableName, CancellationToken cancellationToken);
    Task<PageResult<TModel>> GetAll(PageQuery query, string tableName, CancellationToken cancellationToken);
    Task SaveAsync(TModel model, string tableName, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteItemRequest request, CancellationToken cancellationToken);
}


