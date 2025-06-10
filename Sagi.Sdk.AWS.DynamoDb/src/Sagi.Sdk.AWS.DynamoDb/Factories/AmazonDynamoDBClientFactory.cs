using Amazon;
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
            ThrottleRetries = false,
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
            dbConfig.RegionEndpoint = configurator.Region;
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
//An exception of type 'Amazon.Runtime.AmazonClientException' occurred in System.Private.CoreLib.dll but was not handled in user code: 'No RegionEndpoint or ServiceURL configured'