using AutoFixture.Idioms;
using NSubstitute;
using Sagi.Sdk.AWS.DynamoDb.Initializers;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Initializers;

public class DatabaseContextInitializerTests
{
    private readonly IDynamoDbTableInitializer _database;
    private readonly DatabaseContextInitializer _sut;

    public DatabaseContextInitializerTests()
    {
        _database = Substitute.For<IDynamoDbTableInitializer>();
        _sut = new DatabaseContextInitializer(_database);
    }

    [Theory, AutoNSubstituteData]
    public void ShouldValidateConstructorParameters(GuardClauseAssertion assertion)
        => assertion.Verify(typeof(DatabaseContextInitializer).GetConstructors());

    [Fact]
    public async Task ExecuteAsync_ShouldCallConfigureAsync()
    {
        var cancellationToken = CancellationToken.None;

        await _sut.StartAsync(cancellationToken);

        await _database.Received().ConfigureAsync();
    }
}