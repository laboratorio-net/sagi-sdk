using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Samples.Tables;

public class SecondTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.SecondTable.Table";
    public SecondTable()
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
        BillingMode = BillingMode.PAY_PER_REQUEST;;
        GlobalSecondaryIndexes = [];
    }
}

