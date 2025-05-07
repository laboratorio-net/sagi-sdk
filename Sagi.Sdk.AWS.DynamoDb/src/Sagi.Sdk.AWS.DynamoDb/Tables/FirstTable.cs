using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Indexes;
using Sagi.Sdk.AWS.DynamoDb.Options;

namespace Sagi.Sdk.AWS.DynamoDb.Tables;

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
            new ("Status", ScalarAttributeType.S),
            new ("CreatedAt", ScalarAttributeType.S),
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

