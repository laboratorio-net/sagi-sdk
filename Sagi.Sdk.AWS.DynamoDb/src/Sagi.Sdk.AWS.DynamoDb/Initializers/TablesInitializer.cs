using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Extensions.NETCore.Setup;

namespace Sagi.Sdk.AWS.DynamoDb.Initializers;


public class DatabaseContextInitializer
{
    //TROCAR POR HOSTED SERVICES
    private readonly IDynamoDbTableInitializer _database;

    public DatabaseContextInitializer(IDynamoDbTableInitializer database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public async Task InitializeAsync()
    {
        await _database.ConfigureAsync();
    }
}

public interface IDynamoDbTableInitializer
{
    Task ConfigureAsync();
    Task<bool> TableExistAsync(string tableName);
}

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

public class DynamoDbOptions : AWSOptions
{
    public required BillingMode BillingMode { get; set; }
}

public class FirstTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.First.Table";
    public FirstTable(DynamoDbOptions options)
    {
        TableName = TABLE_NAME;
        AttributeDefinitions =
        [
            new ("PartitionKey", ScalarAttributeType.S),
            new ("SortKey", ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new ("PartitionKey", KeyType.HASH),
            new ("SortKey", KeyType.RANGE),
        ];
        BillingMode = options.BillingMode;
        GlobalSecondaryIndexes = [
            new GetByStatusIndex(),
        ];
    }
}

public class SecondTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.SecondTable.Table";
    public SecondTable(DynamoDbOptions options)
    {
        TableName = TABLE_NAME;
        AttributeDefinitions =
        [
            new ("PartitionKey", ScalarAttributeType.S),
            new ("SortKey", ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new ("PartitionKey", KeyType.HASH),
            new ("SortKey", KeyType.RANGE),
        ];
        BillingMode = options.BillingMode;
        GlobalSecondaryIndexes = [];
    }
}

public class GetByStatusIndex : GlobalSecondaryIndex
{
    public const string INDEX_NAME = "GET_BY_STATUS";

    public GetByStatusIndex()
    {
        IndexName = INDEX_NAME;
        KeySchema = [
            new("Status", KeyType.HASH),
            new("CreatedAt", KeyType.RANGE),
        ];

        Projection = new()
        {
            ProjectionType = ProjectionType.INCLUDE,
            NonKeyAttributes =
            [
                "PartitionKey",
                "Status",
                "CreatedAt",
            ]
        };
    }
}

