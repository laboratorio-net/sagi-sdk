using Sagi.Sdk.MongoDb.Context;

namespace Sagi.Sdk.MongoDb.Tests;

public class DocumentTest
{
    [Fact]
    public void NewDocument_ShouldHaveNonNullId()
    {
        var document = new Document();

        Assert.NotNull(document.Id);
        Assert.False(string.IsNullOrWhiteSpace(document.Id));
    }

    [Fact]
    public void NewId_ShouldGenerateUniqueIds()
    {
        var id1 = Document.NewId();
        var id2 = Document.NewId();

        Assert.NotNull(id1);
        Assert.NotNull(id2);
        Assert.NotEqual(id1, id2);
    }
}