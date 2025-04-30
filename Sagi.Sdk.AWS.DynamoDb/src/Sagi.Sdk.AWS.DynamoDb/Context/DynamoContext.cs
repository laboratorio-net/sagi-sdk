using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.AWS.DynamoDb.Context;

public interface IDynamoDbContext<TModel> where TModel : class
{
    Task<TModel?> GetSingleAsync(QueryFilter filter, string tableName, CancellationToken cancellationToken);
    Task<PageResult<TModel>> GetAll(PageQuery query, string tableName, CancellationToken cancellationToken);
    Task SaveAsync(TModel model, string tableName, CancellationToken cancellationToken);
    Task DeleteAsync(DeleteItemRequest request, CancellationToken cancellationToken);
}

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

    public Task DeleteAsync(DeleteItemRequest request, CancellationToken cancellationToken)
    {
        return _client.DeleteItemAsync(request, cancellationToken);
    }

    public async Task<PageResult<TModel>> GetAll(PageQuery query, string tableName, CancellationToken cancellationToken)
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

    public async Task<TModel?> GetSingleAsync(QueryFilter filter, string tableName, CancellationToken cancellationToken)
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

    public Task SaveAsync(TModel model, string tableName, CancellationToken cancellationToken)
    {
        return _context.SaveAsync(model, CreateConfig(tableName), cancellationToken);
    }

    private static DynamoDBOperationConfig CreateConfig(string tableName) =>
        new() { OverrideTableName = tableName };
}

public class PageResult<TResult> where TResult : class
{
    public IEnumerable<TResult> Items { get; set; } = [];
    public string? PageToken { get; set; }
    public bool HasNextPage => !string.IsNullOrEmpty(PageToken);
}

public class PageQuery
{
    public const int MAX_PAGE_SIZE = 100;
    public const int MIN_PAGE_SIZE = 1;

    public int PageSize { get; set; } = MIN_PAGE_SIZE;
    public string? PageToken { get; set; }

    public bool IsValid => PageSize >= 1 && PageSize <= MAX_PAGE_SIZE;
    public bool IsInvalid => !IsValid;
    public static Error InvalidPageSize => new($"INVALID_PAGE_SIZE",
        $"The PageSize field must have a value between {MIN_PAGE_SIZE} and {MAX_PAGE_SIZE}.");
}

public static class DynamoDbAttributeValueExtensions
{
    public static JsonSerializerOptions JsonOptions
        => new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    public static Dictionary<string, AttributeValue>? Deserialize(this string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return default;
        }

        var bytes = Convert.FromBase64String(token);
        return JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(bytes)!;
    }

    public static string? Serialize(this Dictionary<string, AttributeValue>? token)
    {
        if (token is { Count: > 0 })
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(token, JsonOptions);
            return Convert.ToBase64String(bytes);
        }

        return null;
    }
}


