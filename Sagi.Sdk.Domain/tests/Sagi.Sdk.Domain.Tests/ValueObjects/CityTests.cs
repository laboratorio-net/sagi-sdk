using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class CityTests
{
    private readonly Country _validCountry = new("Brazil", "BR", "BRA");
    private readonly State _validState;

    public CityTests()
    {
        _validState = new State("Rio Grande do Norte", "RN", _validCountry);
        _validState.Validate();
    }

    [Fact]
    public void City_ShouldBeValid_WhenPropertiesAreValid()
    {
        var sut = new City("Natal", _validState);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("Natal", sut.Name);
        Assert.Equal(_validState, sut.State);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void City_ShouldBeInvalid_WhenNameIsInvalid(string name)
    {
        var sut = new City(name, _validState);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void City_ShouldBeInvalid_WhenStateIsNull()
    {
        var sut = new City("Natal", null);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void City_ShouldBeInvalid_WhenStateIsInvalid()
    {
        var invalidCountry = new Country("", "B", "B");
        invalidCountry.Validate();

        var invalidState = new State("", "R", invalidCountry);
        invalidState.Validate();

        var sut = new City("Natal", invalidState);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenCityIsValid()
    {
        var result = City.TryParse("Natal", _validState, out var city);

        Assert.True(result);
        Assert.True(city.IsValid);
        Assert.Equal("Natal", city.Name);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenCityIsInvalid()
    {
        var result = City.TryParse("", null, out var city);

        Assert.False(result);
        Assert.True(city.IsInvalid);
        Assert.NotEmpty(city.Errors);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenCitiesAreEqual()
    {
        var a = new City("Natal", _validState);
        var b = new City("Natal", _validState);

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenCitiesAreDifferent()
    {
        var a = new City("Natal", _validState);
        var b = new City("Mossoró", _validState);

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var sut = new City("Natal", _validState);
        var result = sut.ToString();

        Assert.Equal("Natal - RN/BR", result);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidCity()
    {
        City sut = ("Natal", _validState);
        Assert.True(sut.IsValid);
        Assert.Equal("Natal", sut.Name);
    }
}
