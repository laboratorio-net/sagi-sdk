using Sagi.Sdk.MongoDb.Tests.Docker;

namespace Sagi.Sdk.MongoDb.Tests.Fixtures;

[CollectionDefinition("Mongo Docker Container")]
public class DockerCollection :
    ICollectionFixture<MongoDockerContainer>
{ }