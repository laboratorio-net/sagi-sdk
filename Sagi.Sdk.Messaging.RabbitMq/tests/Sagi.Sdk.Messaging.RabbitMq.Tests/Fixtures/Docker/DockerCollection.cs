namespace Sagi.Sdk.Messaging.RabbitMq.Tests.Fixtures.Docker;

[CollectionDefinition("Rabbit Docker Container")]
public class DockerCollection :
    ICollectionFixture<RabbitDockerContainer>
{ }