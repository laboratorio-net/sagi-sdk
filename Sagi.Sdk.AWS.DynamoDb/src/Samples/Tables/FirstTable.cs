using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Sagi.Sdk.AWS.DynamoDb.Indexes;

namespace Samples.Tables;

public class FirstTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.First.Table";
    public FirstTable()
    {
        TableName = TABLE_NAME;
        AttributeDefinitions =
        [
            new ("PartitionKey", ScalarAttributeType.S),
            new ("SortKey", ScalarAttributeType.S),
            new("CreatedAt", ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new ("PartitionKey", KeyType.HASH),
            new ("SortKey", KeyType.RANGE),
        ];
        BillingMode = BillingMode.PAY_PER_REQUEST;
        GlobalSecondaryIndexes = [
            new GetByStatusIndex(),
        ];
    }
}

