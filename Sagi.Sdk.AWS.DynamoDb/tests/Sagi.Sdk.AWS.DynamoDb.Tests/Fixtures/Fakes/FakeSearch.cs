using Amazon.DynamoDBv2.DataModel;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

public class FakeSearch : AsyncSearch<FakeModel>
{
    public override Task<List<FakeModel>> GetNextSetAsync(
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new List<FakeModel>()
        {
            new() { Id = "1" }
        });
    }
}