using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Initializers;

namespace Sagi.Sdk.AWS.DynamoDb.Extensions;

public static class DynamoConfigExtensions
{
    public static IServiceCollection AddDynamoDb(this IServiceCollection services,
        Action<DynamoDbConfigurator> action)
    {
        var configurator = new DynamoDbConfigurator();
        action(configurator);

        var dbConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = configurator.ServiceURL,
            MaxErrorRetry = 10,
            ThrottleRetries = false
        };

        var client = new AmazonDynamoDBClient(
            configurator.Accesskey,
            configurator.SecretKey,
            // options.SessionToken,
            dbConfig
        );

        services.AddSingleton(configurator);
        services.AddSingleton<IAmazonDynamoDB>(client);
        services.AddSingleton<IDynamoDBContext>(new DynamoDBContext(client));
        
        if (configurator.InitializeDb)
        {
            services.AddSingleton<IDynamoDbTableInitializer>(x =>
            {
                var initializer = new TablesInitializer(client);
                initializer.AddTable([.. configurator.Tables]);
                return initializer;
            });

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

<<<<<<< HEAD
        services.AddSingleton<IAmazonDynamoDB>(client);
        services.AddAWSService<IAmazonDynamoDB>(new DynamoDbOptions());
=======
        services.AddAWSService<IAmazonDynamoDB>(new DynamoDbConfigurator());
>>>>>>> d52ef19417d8a2fe522571758fe84d58d3cfe6a4
        services.AddSingleton<IDynamoDBContext>(new DynamoDBContext(client));

        return services;
    }
}