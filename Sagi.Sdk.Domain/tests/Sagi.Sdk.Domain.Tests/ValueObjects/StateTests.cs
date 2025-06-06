using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class StateTests
{
    private readonly Country _validCountry = new("Brazil", "BR", "BRA");

    [Fact]
    public void State_ShouldBeValid_WhenPropertiesAreValid()
    {
        var sut = new State("Rio Grande do Norte", "RN", _validCountry);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("Rio Grande do Norte", sut.Name);
        Assert.Equal("RN", sut.Abbreviation);
        Assert.Equal(_validCountry, sut.Country);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void State_ShouldBeInvalid_WhenNameIsEmpty(string name)
    {
        var sut = new State(name, "RN", _validCountry);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("R")]
    [InlineData("RNE")]
    public void State_ShouldBeInvalid_WhenAbbreviationIsInvalid(string abbreviation)
    {
        var sut = new State("Rio Grande do Norte", abbreviation, _validCountry);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void State_ShouldBeInvalid_WhenCountryIsNull()
    {
        var sut = new State("Rio Grande do Norte", "RN", null);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void State_ShouldBeInvalid_WhenCountryIsInvalid()
    {
        var invalidCountry = new Country("", "B", "B");
        var sut = new State("Rio Grande do Norte", "RN", invalidCountry);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrueAndParse_WhenValuesAreValid()
    {
        var result = State.TryParse("Rio Grande do Norte", "RN", _validCountry, out var state);

        Assert.True(result);
        Assert.True(state.IsValid);
        Assert.Equal("RN", state.Abbreviation);
        Assert.Equal(_validCountry, state.Country);
    }

    [Fact]
    public void TryParse_ShouldReturnFalseAndNotParse_WhenValuesAreInvalid()
    {
        var result = State.TryParse("", "R", null, out var state);

        Assert.False(result);
        Assert.True(state.IsInvalid);
        Assert.NotEmpty(state.Errors);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenStatesAreEqual()
    {
        var a = new State("Rio Grande do Norte", "RN", _validCountry);
        var b = new State("Rio Grande do Norte", "RN", _validCountry);

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenStatesAreDifferent()
    {
        var a = new State("Rio Grande do Norte", "RN", _validCountry);
        var b = new State("São Paulo", "SP", _validCountry);

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var state = new State("Rio Grande do Norte", "rn", _validCountry);
        var result = state.ToString();

        Assert.Equal("Rio Grande do Norte (RN) - BR", result);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidState()
    {
        State sut = ("Rio Grande do Norte", "RN", _validCountry);
        Assert.True(sut.IsValid);
        Assert.Equal("RN", sut.Abbreviation);
    }
}