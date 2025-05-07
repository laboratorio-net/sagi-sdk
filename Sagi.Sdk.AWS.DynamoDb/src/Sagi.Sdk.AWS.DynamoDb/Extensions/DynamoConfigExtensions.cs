using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Initializers;
using Sagi.Sdk.AWS.DynamoDb.Options;

namespace Sagi.Sdk.AWS.DynamoDb.Extensions;

public static class DynamoConfigExtensions
{
    public static IServiceCollection AddDynamoDb(this IServiceCollection services,
        Action<DynamoDbOptions> action)
    {
        var options = new DynamoDbOptions();
        action(options);

        var config = new AmazonDynamoDBConfig
        {
            ServiceURL = options.ServiceURL,
            MaxErrorRetry = 10,
            ThrottleRetries = false
        };

        var client = new AmazonDynamoDBClient(
            options.Accesskey,
            options.SecretKey,
            // options.SessionToken,
            config
        );

        services.AddSingleton(options);
        services.AddSingleton(client);
        services.AddSingleton<IAmazonDynamoDB>(client);
        services.AddSingleton<IDynamoDBContext>(new DynamoDBContext(client));
        services.AddSingleton<IDynamoDbTableInitializer, TablesInitializer>();

        if (options.InitializeDb)
        {
            services.AddHostedService<DatabaseContextInitializer>();
        }

        return services;
    }

    public static IServiceCollection AddDynamoDb(this IServiceCollection services)
    {
        var config = new AmazonDynamoDBConfig
        {
            MaxErrorRetry = 10,
            ThrottleRetries = false
        };

        var client = new AmazonDynamoDBClient(config);
        services.AddSingleton(client);

        services.AddSingleton<IAmazonDynamoDB>(client);
        services.AddAWSService<IAmazonDynamoDB>(new DynamoDbOptions());
        services.AddSingleton<IDynamoDBContext>(new DynamoDBContext(client));

        return services;
    }
}