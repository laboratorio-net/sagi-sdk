using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class AddressTests
{
    private readonly Country _country = new("Brazil", "BR", "BRA");
    private readonly State _state;
    private readonly City _city;
    private readonly Neighborhood _neighborhood;
    private readonly ZipCode _zipCode;

    public AddressTests()
    {
        _state = new State("Rio Grande do Norte", "RN", _country);
        _city = new City("Natal", _state);
        _neighborhood = new Neighborhood("Tirol", _city);
        _zipCode = new ZipCode("59020010");
    }

    [Fact]
    public void Address_ShouldBeValid_WhenAllFieldsAreCorrect()
    {
        var sut = new Address("Av. Afonso Pena", "123", "Apto 101", _neighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("Av. Afonso Pena", sut.Street);
        Assert.Equal("123", sut.Number);
        Assert.Equal("Apto 101", sut.Complement);
        Assert.Equal(_neighborhood, sut.Neighborhood);
        Assert.Equal(_zipCode, sut.ZipCode);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("A")]
    public void Address_ShouldBeInvalid_WhenStreetIsTooShort(string street)
    {
        var sut = new Address(street, "123", null, _neighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Address_ShouldBeInvalid_WhenStreetIsTooLong()
    {
        var street = new string('a', 81);
        var sut = new Address(street, "123", null, _neighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Address_ShouldBeInvalid_WhenNumberIsEmptyOrNull(string number)
    {
        var sut = new Address("Rua das Flores", number, null, _neighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Address_ShouldBeInvalid_WhenNumberIsTooLong()
    {
        var number = new string('9', 11);
        var sut = new Address("Rua das Flores", number, null, _neighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Address_ShouldBeInvalid_WhenNeighborhoodIsInvalid()
    {
        var invalidCity = new City("", _state);
        invalidCity.Validate();

        var invalidNeighborhood = new Neighborhood("", invalidCity);
        invalidNeighborhood.Validate();

        var sut = new Address("Rua das Flores", "123", null, invalidNeighborhood, _zipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Address_ShouldBeInvalid_WhenZipCodeIsInvalid()
    {
        var invalidZipCode = new ZipCode("");
        invalidZipCode.Validate();

        var sut = new Address("Rua das Flores", "123", null, _neighborhood, invalidZipCode);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenAddressesAreEqual()
    {
        var a = new Address("Rua das Flores", "123", "Apto 2", _neighborhood, _zipCode);
        var b = new Address("Rua das Flores", "123", "Apto 2", _neighborhood, _zipCode);

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenAddressesAreDifferent()
    {
        var a = new Address("Rua das Flores", "123", "Apto 2", _neighborhood, _zipCode);
        var b = new Address("Rua das Flores", "456", "Casa", _neighborhood, _zipCode);

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void ToString_ShouldReturnFormattedAddress()
    {
        var sut = new Address("Av. Prudente de Morais", "12B", "Fundos", _neighborhood, _zipCode);
        var expected = $"Av. Prudente de Morais, 12B - Tirol, Natal - RN/BR - ZIP: {_zipCode}";

        Assert.Equal(expected, sut.ToString());
    }
}