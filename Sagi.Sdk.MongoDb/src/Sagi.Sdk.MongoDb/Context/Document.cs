using MongoDB.Bson;

namespace Sagi.Sdk.MongoDb.Context;

public class Document
{
    public string Id { get; set; } = NewId();

    public static string NewId() 
        => ObjectId.GenerateNewId().ToString();
}