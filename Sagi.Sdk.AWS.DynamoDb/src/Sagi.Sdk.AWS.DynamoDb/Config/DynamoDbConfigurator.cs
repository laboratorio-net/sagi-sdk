using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Extensions.NETCore.Setup;

namespace Sagi.Sdk.AWS.DynamoDb.Config;

public class DynamoDbConfigurator : AWSOptions
{
    private readonly List<CreateTableRequest> _tables = [];

    public DynamoDbConfigurator()
    {
        Region = RegionEndpoint.USEast1;        
    }

    public BillingMode BillingMode { get; set; } = BillingMode.PAY_PER_REQUEST;
    public string? Accesskey { get; set; }
    public string? SecretKey { get; set; }
    public string? SessionToken { get; set; }
    public string? ServiceURL { get; set; }
    public bool InitializeDb { get; set; }

    internal bool UseAWSService =>
        string.IsNullOrWhiteSpace(Accesskey) &&
        string.IsNullOrWhiteSpace(SecretKey) &&
        string.IsNullOrWhiteSpace(SessionToken);

    public IReadOnlyList<CreateTableRequest> Tables => _tables;

    public void ConfigureTable(CreateTableRequest table)
    {
        ArgumentNullException.ThrowIfNull(table);
        _tables.Add(table);
    }
}