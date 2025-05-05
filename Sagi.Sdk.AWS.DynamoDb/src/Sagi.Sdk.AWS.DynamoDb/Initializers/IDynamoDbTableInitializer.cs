namespace Sagi.Sdk.AWS.DynamoDb.Initializers;

public interface IDynamoDbTableInitializer
{
    Task ConfigureAsync();
    Task<bool> TableExistAsync(string tableName);
}