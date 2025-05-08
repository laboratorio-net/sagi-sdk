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
            new (nameof(Order.Id), ScalarAttributeType.S),
            new(nameof(Order.CreatedAt), ScalarAttributeType.S),
            new(nameof(Order.Status), ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new (nameof(Order.Id), KeyType.HASH),
            new (nameof(Order.CreatedAt), KeyType.RANGE),
        ];
        BillingMode = BillingMode.PAY_PER_REQUEST;
        GlobalSecondaryIndexes = [
            new GetByStatusIndex(),
        ];
    }
}