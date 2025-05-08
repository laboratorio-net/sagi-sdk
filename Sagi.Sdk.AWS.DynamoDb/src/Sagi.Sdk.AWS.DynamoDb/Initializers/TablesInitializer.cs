using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Sagi.Sdk.AWS.DynamoDb.Initializers;

public class TablesInitializer : IDynamoDbTableInitializer
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly List<CreateTableRequest> _tables;

    public TablesInitializer(IAmazonDynamoDB dynamoDb)
    {
        _dynamoDb = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
        _tables = [];
    }

    public void AddTable(CreateTableRequest[] tables)
    {
        if (tables.Length > 0)
        {
            _tables.AddRange(tables);
        }
    }

    public async Task ConfigureAsync()
    {
        foreach (var table in _tables)
        {
            await CreateIfNotExistAsync(table);
        }
    }

    private async Task CreateIfNotExistAsync(CreateTableRequest request)
    {
        if ((await TableExistAsync(request.TableName)) == false)
        {
            await _dynamoDb.CreateTableAsync(request);
        }
    }

    private async Task<bool> TableExistAsync(string tableName)
    {
        var tables = await _dynamoDb.ListTablesAsync();
        var existTable = tables.TableNames.Contains(tableName);
        return existTable;
    }
}