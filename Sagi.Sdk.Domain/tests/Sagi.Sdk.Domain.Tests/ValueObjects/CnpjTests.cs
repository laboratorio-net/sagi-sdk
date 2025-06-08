using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class CnpjTests
{
    [Theory]
    [InlineData("45.723.174/0001-10")]
    [InlineData("45723174000110")]
    public void Cnpj_ShouldBeValid_WhenNumberIsValid(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("45723174000110", sut.Number);
    }

    [Theory]
    [InlineData("00.000.000/0000-00")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    [InlineData("33333333333333")]
    [InlineData("12345678901234")]
    public void Cnpj_ShouldBeInvalid_WhenNumberFailsCheckDigit(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Cnpj_ShouldBeInvalid_WhenNumberIsNullOrEmpty(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789012345")] // 15 digits
    [InlineData("abcdefg1234567")] // non-numeric
    public void Cnpj_ShouldBeInvalid_WhenFormatIsIncorrect(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenCnpjIsValid()
    {
        var result = Cnpj.TryParse("45723174000110", out var cnpj);

        Assert.True(result);
        Assert.True(cnpj.IsValid);
        Assert.Equal("45723174000110", cnpj.Number);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenCnpjIsInvalid()
    {
        var result = Cnpj.TryParse("12345678901234", out var cnpj);

        Assert.False(result);
        Assert.True(cnpj.IsInvalid);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidCnpj()
    {
        Cnpj cnpj = "45723174000110";
        Assert.True(cnpj.IsValid);
        Assert.Equal("45723174000110", cnpj.Number);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameCnpjNumbers()
    {
        var a = new Cnpj("45723174000110");
        var b = new Cnpj("45.723.174/0001-10");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentCnpjNumbers()
    {
        var a = new Cnpj("45723174000110");
        var b = new Cnpj("12345678000199");

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Formatted_ShouldReturnFormattedCnpj()
    {
        var sut = new Cnpj("45723174000110");
        Assert.Equal("45.723.174/0001-10", sut.Formatted);
    }

    [Fact]
    public void Formatted_ShouldReturnEmpty_WhenNumberIsInvalid()
    {
        var sut = new Cnpj("");
        Assert.Equal(string.Empty, sut.Formatted);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedCnpj()
    {
        var sut = new Cnpj("45723174000110");
        Assert.Equal(sut.Formatted, sut.ToString());
    }
}