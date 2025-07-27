namespace Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures.Docker;

[CollectionDefinition("DynamoDb Docker Container")]
public class DockerCollection :
    ICollectionFixture<DynamoDbDockerContainer>
{ }