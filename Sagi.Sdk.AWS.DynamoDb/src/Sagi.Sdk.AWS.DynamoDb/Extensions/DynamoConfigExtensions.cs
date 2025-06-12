using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Microsoft.Extensions.DependencyInjection;

using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Context;
using Sagi.Sdk.AWS.DynamoDb.Factories;
using Sagi.Sdk.AWS.DynamoDb.Initializers;

namespace Sagi.Sdk.AWS.DynamoDb.Extensions;

public static class DynamoConfigExtensions
{
    public static IServiceCollection AddDynamoDb(this IServiceCollection services,
        Action<DynamoDbConfigurator> action)
    {
        var configurator = new DynamoDbConfigurator();
        action(configurator);

        if (configurator.UseAWSService)
        {
            services.AddAWSService<IAmazonDynamoDB>(configurator);
        }
        else
        {
            var client = AmazonDynamoDBClientFactory.Create(configurator);
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext>(new DynamoDBContext(client));
        }

        services.AddSingleton(typeof(IDynamoDbContext<>), typeof(DynamoDbContext<>));

        if (configurator.InitializeDb)
        {
            services.AddSingleton<IDynamoDbTableInitializer>(x =>
            {
                var initializer = new TablesInitializer(x.GetService<IAmazonDynamoDB>()!);
                initializer.AddTable([.. configurator.Tables]);
                return initializer;
            });

            services.AddHostedService<DatabaseContextInitializer>();
        }

        return services;
    }
}