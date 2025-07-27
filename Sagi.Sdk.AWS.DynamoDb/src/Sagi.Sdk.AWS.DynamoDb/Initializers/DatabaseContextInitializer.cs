using Microsoft.Extensions.Hosting;

namespace Sagi.Sdk.AWS.DynamoDb.Initializers;

public class DatabaseContextInitializer : BackgroundService
{
    private readonly IDynamoDbTableInitializer _database;

    public DatabaseContextInitializer(IDynamoDbTableInitializer database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _database.ConfigureAsync();
    }
}