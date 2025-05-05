using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Options;

namespace Sagi.Sdk.AWS.DynamoDb.Tables;

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

