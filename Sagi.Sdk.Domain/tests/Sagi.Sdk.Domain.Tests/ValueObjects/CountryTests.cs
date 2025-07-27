using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class CountryTests
{
    [Fact]
    public void Country_ShouldBeValid_WhenPropertiesAreValid()
    {
        var sut = new Country("Brazil", "BR", "BRA");
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("Brazil", sut.Name);
        Assert.Equal("BR", sut.Abbreviation);
        Assert.Equal("BRA", sut.IsoCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Country_ShouldBeInvalid_WhenNameIsEmpty(string name)
    {
        var sut = new Country(name, "BR", "BRA");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("B")]
    [InlineData("BRA")]
    public void Country_ShouldBeInvalid_WhenAbbreviationIsInvalid(string abbreviation)
    {
        var sut = new Country("Brazil", abbreviation, "BRA");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("BR")]
    [InlineData("BRAX")]
    public void Country_ShouldBeInvalid_WhenIsoCodeIsInvalid(string isoCode)
    {
        var sut = new Country("Brazil", "BR", isoCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrueAndParse_WhenValuesAreValid()
    {
        var result = Country.TryParse("Brazil", "BR", "BRA", out var country);

        Assert.True(result);
        Assert.True(country.IsValid);
        Assert.Equal("Brazil", country.Name);
        Assert.Equal("BR", country.Abbreviation);
        Assert.Equal("BRA", country.IsoCode);
    }

    [Fact]
    public void TryParse_ShouldReturnFalseAndNotParse_WhenValuesAreInvalid()
    {
        var result = Country.TryParse("", "B", "B", out var country);

        Assert.False(result);
        Assert.True(country.IsInvalid);
        Assert.NotEmpty(country.Errors);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenCountriesAreEqual()
    {
        var a = new Country("Brazil", "BR", "BRA");
        var b = new Country("Brazil", "BR", "BRA");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenCountriesAreDifferent()
    {
        var a = new Country("Brazil", "BR", "BRA");
        var b = new Country("Argentina", "AR", "ARG");

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var country = new Country("Brazil", "br", "bra");
        var result = country.ToString();

        Assert.Equal("Brazil (BR/BRA)", result);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidCountry()
    {
        Country sut = ("Brazil", "BR", "BRA");
        Assert.True(sut.IsValid);
        Assert.Equal("Brazil", sut.Name);
    }
}