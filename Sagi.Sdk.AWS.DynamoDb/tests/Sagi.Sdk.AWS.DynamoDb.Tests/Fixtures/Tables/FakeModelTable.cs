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
        ];
        KeySchema =
        [
            new (nameof(FakeModel.Id), KeyType.HASH),
        ];
        BillingMode = BillingMode.PAY_PER_REQUEST;
        GlobalSecondaryIndexes = [];
    }

    protected abstract string GetTableName();
}

internal class GetSingleTestTable : FakeModelTable
{
    public const string TABLE_NAME = "FakeModel.GetSingle.Tests";
    protected override string GetTableName() => TABLE_NAME;
}

internal class GetAllTestTable : FakeModelTable
{
    public const string TABLE_NAME = "FakeModel.GetAll.Tests";
    protected override string GetTableName() => TABLE_NAME;
}

internal class InsertTestTable : FakeModelTable
{
    public const string TABLE_NAME = "FakeModel.Insert.Tests";
    protected override string GetTableName() => TABLE_NAME;
}


internal class DeleteTestTable : FakeModelTable
{
    public const string TABLE_NAME = "FakeModel.Delete.Tests";
    protected override string GetTableName() => TABLE_NAME;
}