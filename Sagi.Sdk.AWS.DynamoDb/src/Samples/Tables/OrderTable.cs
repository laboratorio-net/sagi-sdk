using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

using Sagi.Sdk.AWS.DynamoDb.Indexes;

using Samples.Entities;

namespace Samples.Tables;

public class OrderTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.Orders";
    public OrderTable()
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