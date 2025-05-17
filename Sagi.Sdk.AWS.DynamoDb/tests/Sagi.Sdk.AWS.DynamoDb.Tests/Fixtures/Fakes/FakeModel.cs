namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Fakes;

public class FakeModel
{
    public const string TABLE_NAME = "fakeTable";
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public FakeModel WithName(string name)
    {
        Name = name;
        return this;
    }
}