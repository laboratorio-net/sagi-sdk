using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace Samples.Tables;

public class PaymentTable : CreateTableRequest
{
    public const string TABLE_NAME = "Sample.Payments";
    
    public PaymentTable()
    {
        TableName = TABLE_NAME;
        AttributeDefinitions =
        [
            new (nameof(Payment.Id), ScalarAttributeType.S),
            new (nameof(Payment.CreatedAt), ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new (nameof(Payment.Id), KeyType.HASH),
            new (nameof(Payment.CreatedAt), KeyType.RANGE),
        ];
        BillingMode = BillingMode.PAY_PER_REQUEST;;
        GlobalSecondaryIndexes = [];
    }
}