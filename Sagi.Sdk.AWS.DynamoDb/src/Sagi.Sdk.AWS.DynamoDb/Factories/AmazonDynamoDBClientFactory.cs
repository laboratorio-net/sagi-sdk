using Amazon.DynamoDBv2;
using Sagi.Sdk.AWS.DynamoDb.Config;

namespace Sagi.Sdk.AWS.DynamoDb.Factories;

public static class AmazonDynamoDBClientFactory
{
    public static AmazonDynamoDBClient Create()
    {
        var config = new AmazonDynamoDBConfig
        {
            MaxErrorRetry = 10,
            ThrottleRetries = false
        };

        var client = new AmazonDynamoDBClient(config);
        return client;
    }

    public static AmazonDynamoDBClient Create(DynamoDbConfigurator configurator)
    {
        var dbConfig = new AmazonDynamoDBConfig
        {
            ServiceURL = configurator.ServiceURL,
            MaxErrorRetry = 10,
            ThrottleRetries = false
        };

        AmazonDynamoDBClient client;

        if (string.IsNullOrEmpty(configurator.SessionToken))
        {
            client = new AmazonDynamoDBClient(
                configurator.Accesskey,
                configurator.SecretKey,
                dbConfig
            );
        }
        else
        {
            client = new AmazonDynamoDBClient(
                configurator.Accesskey,
                configurator.SecretKey,
                configurator.SessionToken,
                dbConfig
            );
        }

        return client;
    }
}