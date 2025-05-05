using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Options;
using Sagi.Sdk.AWS.DynamoDb.Tables;

namespace Sagi.Sdk.AWS.DynamoDb.Initializers;

public class TablesInitializer : IDynamoDbTableInitializer
{
    private readonly IAmazonDynamoDB _dynamoDb;
    private readonly DynamoDbOptions _options;

    public TablesInitializer(
        IAmazonDynamoDB dynamoDb,
        DynamoDbOptions options)
    {
        _dynamoDb = dynamoDb ?? throw new ArgumentNullException(nameof(dynamoDb));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task ConfigureAsync()
    {
        await Task.WhenAll(
            CreateFirstTableAsync(),
            CreateSecondTableAsync()
        );
    }

    private async Task CreateIfNotExistAsync(CreateTableRequest request)
    {
        if (await TableExistAsync(request.TableName)) { return; }
        await _dynamoDb.CreateTableAsync(request);
    }

    public async Task<bool> TableExistAsync(string tableName)
    {
        var tables = await _dynamoDb.ListTablesAsync();
        var existTable = tables.TableNames.Contains(tableName);
        return existTable;
    }

    private async Task CreateFirstTableAsync()
    {
        var request = new FirstTable(_options);
        await CreateIfNotExistAsync(request);
    }

    private async Task CreateSecondTableAsync()
    {
        var request = new SecondTable(_options);
        await CreateIfNotExistAsync(request);
    }
}

