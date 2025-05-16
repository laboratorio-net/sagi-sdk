using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Tables;

public abstract class FakeModelTable : CreateTableRequest
{
    public FakeModelTable()
    {
        TableName = GetTableName();
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

    protected abstract string GetTableName();
}

internal class InsertTestTable : FakeModelTable
{
    public const string TABLE_NAME = "FakeModel.Insert.Tests";
    protected override string GetTableName() => TABLE_NAME;
}