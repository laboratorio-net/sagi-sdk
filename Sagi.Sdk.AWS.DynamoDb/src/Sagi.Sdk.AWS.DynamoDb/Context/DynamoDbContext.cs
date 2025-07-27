using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;

using Sagi.Sdk.AWS.DynamoDb.Extensions;
using Sagi.Sdk.AWS.DynamoDb.Pages;

namespace Sagi.Sdk.AWS.DynamoDb.Context;

public class DynamoDbContext<TModel> : IDynamoDbContext<TModel> where TModel : class
{
    private readonly IDynamoDBContext _context;
    private readonly IAmazonDynamoDB _client;

    public DynamoDbContext(
        IDynamoDBContext context,
        IAmazonDynamoDB client)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }    

    public async Task<TModel?> GetSingleAsync(
        QueryFilter filter, string tableName,
        CancellationToken cancellationToken)
    {
        var query = new QueryOperationConfig
        {
            Filter = filter,
            Limit = 1,
            Select = SelectValues.AllAttributes,
        };

        var search = _context.FromQueryAsync<TModel>(query, CreateConfig(tableName));

        var result = await search.GetNextSetAsync(cancellationToken);
        return result.FirstOrDefault();
    }

    public async Task<PageResult<TModel>> GetAll(
        PageQuery query, string tableName,
        CancellationToken cancellationToken)
    {
        var request = new ScanRequest
        {
            TableName = tableName,
            Limit = query.PageSize,
            Select = Select.ALL_ATTRIBUTES,
            ExclusiveStartKey = query.PageToken.Deserialize(),
        };

        var response = await _client.ScanAsync(request, cancellationToken);

        var items = response.Items
           .Select(Document.FromAttributeMap)
           .Select(_context.FromDocument<TModel>);

        return new PageResult<TModel>
        {
            Items = items,
            PageToken = response.LastEvaluatedKey.Serialize(),
        };
    }

    public Task SaveAsync(
        TModel model, string tableName,
        CancellationToken cancellationToken)
    {
        return _context.SaveAsync(model, CreateConfig(tableName), cancellationToken);
    }    

    public Task DeleteAsync(
        Dictionary<string, AttributeValue> conditions,
        string tableName, CancellationToken cancellationToken)
    {
        var request = new DeleteItemRequest
        {
            TableName = tableName,
            Key = conditions,
        };

        return _client.DeleteItemAsync(request, cancellationToken);
    }

    private static DynamoDBOperationConfig CreateConfig(string tableName) =>
        new() { OverrideTableName = tableName };
}