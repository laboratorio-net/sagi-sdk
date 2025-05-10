using Sagi.Sdk.AWS.DynamoDb.Pages;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.Pages;

public class PageQueryTests
{
    [Fact]
    public void IsValid_ShouldBeTrue_WhenPageSizeIsValid()
    {
        var sut = new PageQuery();
        Assert.True(sut.IsValid);
        Assert.False(sut.IsInvalid);
    }

    [Fact]
    public void IsValid_ShouldBeFalse_WhenPageSizeIsLessThanMinPageSize()
    {
        var sut = new PageQuery
        {
            PageSize = PageQuery.MIN_PAGE_SIZE - 1,
        };

        Assert.False(sut.IsValid);
        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void IsValid_ShouldBeFalse_WhenPageSizeIsGreatherThaMaxPageSize()
    {
        var sut = new PageQuery
        {
            PageSize = PageQuery.MAX_PAGE_SIZE + 1,
        };

        Assert.False(sut.IsValid);
        Assert.True(sut.IsInvalid);
    }
}