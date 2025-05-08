using Amazon.DynamoDBv2.Model;

namespace Sagi.Sdk.AWS.DynamoDb.Initializers;

public interface IDynamoDbTableInitializer
{
    void AddTable(CreateTableRequest[] tables);
    Task ConfigureAsync();
}