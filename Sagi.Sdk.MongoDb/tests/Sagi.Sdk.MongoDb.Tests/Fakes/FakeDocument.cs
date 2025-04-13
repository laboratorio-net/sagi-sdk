using Sagi.Sdk.MongoDb.Context;

namespace Sagi.Sdk.MongoDb.Tests.Fakes;

public class FakeDocument : Document
{
    public string Foo { get; set; } = "Bar";

    public override bool Equals(object? obj)
    {
        var compareTo = obj as FakeDocument;
        return Id == compareTo?.Id &&
            Foo == compareTo.Foo;
    }

    public override int GetHashCode()
        => (GetType().GetHashCode() * 907) + Id.GetHashCode();
}