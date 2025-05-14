using Sagi.Sdk.AWS.DynamoDb.Pages;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Pages;

public class PageResultTests
{
    [Fact]
    public void HasNextPage_ShouldBeTrue_WhenPageTokenHasValue()
    {
        var sut = new PageResult<string>
        {
            PageToken = Guid.NewGuid().ToString()
        };

        Assert.True(sut.HasNextPage);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void HasNextPage_ShouldBeFalse_WhenPageTokenIsNull(string token)
    {
        var sut = new PageResult<string>
        {
            PageToken = token
        };

        Assert.False(sut.HasNextPage);
    }
}