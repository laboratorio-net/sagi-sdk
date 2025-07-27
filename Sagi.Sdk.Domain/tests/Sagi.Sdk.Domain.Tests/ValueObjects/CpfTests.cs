using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("111.444.777-35")]
    [InlineData("11144477735")]
    public void Cpf_ShouldBeValid_WhenNumberIsValid(string number)
    {
        var sut = new Cpf(number);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.Equal("11144477735", sut.Number);
    }

    [Theory]
    [InlineData("000.000.000-00")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    [InlineData("12345678900")]
    public void Cpf_ShouldBeInvalid_WhenNumberFailsCheckDigit(string number)
    {
        var sut = new Cpf(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("     ")]
    public void Cpf_ShouldBeInvalid_WhenNumberIsNullOrEmpty(string number)
    {
        var sut = new Cpf(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789012")] // 12 digits
    [InlineData("abcdefghijk")] // non-numeric
    public void Cpf_ShouldBeInvalid_WhenFormatIsIncorrect(string number)
    {
        var sut = new Cpf(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenCpfIsValid()
    {
        var result = Cpf.TryParse("11144477735", out var cpf);

        Assert.True(result);
        Assert.True(cpf.IsValid);
        Assert.Equal("11144477735", cpf.Number);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenCpfIsInvalid()
    {
        var result = Cpf.TryParse("12345678900", out var cpf);

        Assert.False(result);
        Assert.True(cpf.IsInvalid);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidCpf()
    {
        Cpf cpf = "11144477735";
        Assert.True(cpf.IsValid);
        Assert.Equal("11144477735", cpf.Number);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameCpfNumbers()
    {
        var a = new Cpf("11144477735");
        var b = new Cpf("111.444.777-35");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentCpfNumbers()
    {
        var a = new Cpf("11144477735");
        var b = new Cpf("22233344450");

        Assert.False(a.Equals(b));
        Assert.False(a.Equals((object)b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Formatted_ShouldReturnFormattedCpf()
    {
        var sut = new Cpf("11144477735");
        Assert.Equal("111.444.777-35", sut.Formatted);
    }

    [Fact]
    public void Formatted_ShouldReturnEmptyStringWhenNumberIsParseble()
    {
        var sut = new Cpf("");
        Assert.Equal(string.Empty, sut.Formatted);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedCpf()
    {
        var sut = new Cpf("11144477735");
        Assert.Equal(sut.Formatted, sut.ToString());
    }
}