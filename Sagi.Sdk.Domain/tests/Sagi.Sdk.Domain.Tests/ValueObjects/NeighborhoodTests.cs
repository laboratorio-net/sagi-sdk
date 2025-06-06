using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class NeighborhoodTests
{
    private readonly Country _validCountry = new("Brazil", "BR", "BRA");
    private readonly State _validState;
    private readonly City _validCity;

    public NeighborhoodTests()
    {
        _validState = new State("Rio Grande do Norte", "RN", _validCountry);
        _validCity = new City("Natal", _validState);
    }

    [Fact]
    public void Neighborhood_ShouldBeValid_WhenPropertiesAreValid()
    {
        var sut = new Neighborhood("Lagoa Nova", _validCity);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("Lagoa Nova", sut.Name);
        Assert.Equal(_validCity, sut.City);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Neighborhood_ShouldBeInvalid_WhenNameIsInvalid(string name)
    {
        var sut = new Neighborhood(name, _validCity);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Neighborhood_ShouldBeInvalid_WhenCityIsNull()
    {
        var sut = new Neighborhood("Lagoa Nova", null);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Neighborhood_ShouldBeInvalid_WhenCityIsInvalid()
    {
        var invalidCountry = new Country("", "B", "B");
        var invalidState = new State("", "R", invalidCountry);
        var invalidCity = new City("", invalidState);

        var sut = new Neighborhood("Lagoa Nova", invalidCity);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenNeighborhoodIsValid()
    {
        var result = Neighborhood.TryParse("Lagoa Nova", _validCity, out var neighborhood);

        Assert.True(result);
        Assert.True(neighborhood.IsValid);
        Assert.Equal("Lagoa Nova", neighborhood.Name);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenNeighborhoodIsInvalid()
    {
        var result = Neighborhood.TryParse("", null, out var neighborhood);

        Assert.False(result);
        Assert.True(neighborhood.IsInvalid);
        Assert.NotEmpty(neighborhood.Errors);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenNeighborhoodsAreEqual()
    {
        var a = new Neighborhood("Lagoa Nova", _validCity);
        var b = new Neighborhood("Lagoa Nova", _validCity);

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenNeighborhoodsAreDifferent()
    {
        var a = new Neighborhood("Lagoa Nova", _validCity);
        var b = new Neighborhood("Tirol", _validCity);

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var sut = new Neighborhood("Lagoa Nova", _validCity);
        var result = sut.ToString();

        Assert.Equal("Lagoa Nova - Natal/RN", result);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidNeighborhood()
    {
        Neighborhood sut = ("Lagoa Nova", _validCity);
        Assert.True(sut.IsValid);
        Assert.Equal("Lagoa Nova", sut.Name);
    }
}