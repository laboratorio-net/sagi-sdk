using Amazon.DynamoDBv2.Model;
using AutoFixture.Idioms;
using Sagi.Sdk.AWS.DynamoDb.Config;
using Sagi.Sdk.AWS.DynamoDb.Tests.Fixtures;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Config;

public class DynamoDbConfiguratorTests
{
    [Theory, AutoNSubstituteData]
    public void ShouldValidateConstructorParameters(GuardClauseAssertion assertion)
        => assertion.Verify(typeof(DynamoDbConfigurator).GetConstructors());

    [Theory, AutoNSubstituteData]
    public void ConfigureTable_ShouldAddTableToList(CreateTableRequest table)
    {
        var sut = new DynamoDbConfigurator();

        sut.ConfigureTable(table);

        Assert.Contains(table, sut.Tables);
    }

    [Theory, AutoNSubstituteData]
    public void ConfigureTable_ShouldThrowIfTableIsNull()
    {
        var sut = new DynamoDbConfigurator();

        Assert.Throws<ArgumentNullException>(() => sut.ConfigureTable(null!));
    }

    [Fact]
    public void Tables_ShouldBeEmptyByDefault()
    {
        var sut = new DynamoDbConfigurator();

        Assert.Empty(sut.Tables);
    }
}