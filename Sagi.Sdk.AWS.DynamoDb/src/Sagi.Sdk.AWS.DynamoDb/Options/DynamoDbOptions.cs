using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;

namespace Sagi.Sdk.AWS.DynamoDb.Options;

public class DynamoDbOptions : AWSOptions
{
    public BillingMode BillingMode { get; set; } = BillingMode.PAY_PER_REQUEST;
    public string? Accesskey { get; set; }
    public string? SecretKey { get; set; }
    public string? SessionToken { get; set; }
    public string? ServiceURL { get; set; }
    public bool InitializeDb { get; set; }
}