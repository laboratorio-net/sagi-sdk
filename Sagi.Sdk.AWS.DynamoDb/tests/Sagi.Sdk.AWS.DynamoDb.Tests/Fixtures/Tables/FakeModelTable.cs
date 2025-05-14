using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Tables;

public class FakeModelTable : CreateTableRequest
{
    public const string TABLE_NAME = "Tests.Fake.Model.Table";

    public FakeModelTable()
    {
        TableName = TABLE_NAME;
        AttributeDefinitions =
        [
            new (nameof(FakeModel.Id), ScalarAttributeType.S),
            new(nameof(FakeModel.CreatedAt), ScalarAttributeType.S),
        ];
        KeySchema =
        [
            new (nameof(FakeModel.Id), KeyType.HASH),
            new (nameof(FakeModel.CreatedAt), KeyType.RANGE),
        ];
        BillingMode = BillingMode.PAY_PER_REQUEST;
        GlobalSecondaryIndexes = [];
    }
}