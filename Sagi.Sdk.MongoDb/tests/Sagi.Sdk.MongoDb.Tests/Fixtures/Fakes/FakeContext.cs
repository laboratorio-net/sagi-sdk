using MongoDB.Driver;

using Sagi.Sdk.MongoDb.Context;

namespace Sagi.Sdk.MongoDb.Tests.Fixtures.Fakes;

public class FakeContext(IMongoDatabase database) :
    MongoContext<FakeDocument>(database)
{
    public override string CollectionName => "FakeContext";
}