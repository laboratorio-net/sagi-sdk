namespace Sagi.Sdk.MongoDb.Tests.Fixtures.Docker;

[CollectionDefinition("Mongo Docker Container")]
public class DockerCollection :
    ICollectionFixture<MongoDockerContainer>
{ }