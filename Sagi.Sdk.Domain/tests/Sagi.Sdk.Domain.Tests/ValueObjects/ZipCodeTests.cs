using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class ZipCodeTests
{
    [Fact]
    public void ZipCode_ShouldThrowArgumentNullException_WhenValueIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ZipCode(null));
    }

    [Fact]
    public void ZipCode_ShouldThrowArgumentNullException_WhenValueIsEmpty()
    {
        Assert.Throws<ArgumentException>(() => new ZipCode(""));
    }

    [Fact]
    public void ZipCode_ShouldBeInvalid_WhenContainsCharacters()
    {
        var sut = new ZipCode("59157A08");
        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("5915740")]
    [InlineData("591574089")]
    public void ZipCode_ShouldBeInvalid_WhenValueLengthIsInvalid(string value)
    {
        var sut = new ZipCode(value);
        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Formatted_ShouldReturn_FormattedZipCode()
    {
        var expected = "59.157-408";
        var sut = new ZipCode("59157408");

        Assert.Equal(expected, sut.Formatted);
    }

    [Fact]
    public void Formatted_ShouldReturnStringEmpty_WhenZipCodeIsNotValid()
    {
        var sut = new ZipCode("59157A08");
        Assert.Empty(sut.Formatted);
    }

    [Fact]
    public void ToString_ShouldReturn_FormattedZipCode()
    {
        var sut = new ZipCode("59157408");
        Assert.Equal(sut.Formatted, sut.ToString());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherIsNull()
    {
        var sut = new ZipCode("59157408");
        ZipCode other = null;

        Assert.False(sut.Equals(other));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenReferences_AreTheSame()
    {
        var sut = new ZipCode("59157408");
        var other = sut;

        Assert.True(sut.Equals(other));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenValuesAreEquals()
    {
        var sut = new ZipCode("59157408");
        var other = new ZipCode("59157408");

        Assert.True(sut.Equals(other));
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_ForEqualZipCodes()
    {
        var sut = new ZipCode("59157408");
        var other = new ZipCode("59157408");

        Assert.Equal(sut.GetHashCode(), other.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferent_ForDifferentZipCodes()
    {
        var sut = new ZipCode("59157408");
        var other = new ZipCode("59157409");

        Assert.NotEqual(sut.GetHashCode(), other.GetHashCode());
    }

    [Fact]
    public void TryParse_ShouldReturnTrueAndParse_WhenValueIsValid()
    {
        var value = "59157408";

        var parsed = ZipCode.TryParse(value, out var zipCode);

        Assert.True(parsed);
        Assert.IsType<ZipCode>(zipCode);
        Assert.True(zipCode.IsValid);
        Assert.Equal(value, zipCode.Value);
    }

    [Fact]
    public void TryParse_ShouldReturnFalseAndNotParse_WhenValueIsInvalid()
    {
        var value = "5915";

        var parsed = ZipCode.TryParse(value, out var zipCode);

        Assert.False(parsed);
        Assert.Null(zipCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void TryParse_ShouldReturnFalseAndNotParse_WhenValueIsNullOrEmpty(string value)
    {
        var parsed = ZipCode.TryParse(value, out var zipCode);

        Assert.False(parsed);
        Assert.Null(zipCode);
    }

    [Fact]
    public void ImplicitOperator_ShouldParse_StringToZipcode()
    {
        ZipCode zipCode = "59157408";
        Assert.True(zipCode?.IsValid);
    }
}