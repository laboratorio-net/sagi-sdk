using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class CnpjTests
{
    // ── Numeric (legacy) ────────────────────────────────────────────────────

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
    [InlineData("78.580.638/0001-36")]
    [InlineData("50.880.859/0001-00")]
    [InlineData("92.372.614/0001-12")]
    [InlineData("32.916.798/0001-02")]
    [InlineData("55.938.308/0001-74")]
    public void Cnpj_ShouldBeValid_WhenNumericCnpjIsValid(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.False(sut.IsAlphanumeric);
    }

    [Theory]
    [InlineData("56.129.990/0001-18")]
    [InlineData("95.960.227/0001-96")]
    [InlineData("80.131.432/0001-71")]
    [InlineData("16.182.448/0001-04")]
    [InlineData("05.303.286/0001-07")]
    public void Cnpj_ShouldBeInvalid_WhenNumericCnpjHasWrongCheckDigit(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
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
    [InlineData("123456789012345")]
    public void Cnpj_ShouldBeInvalid_WhenLengthIsIncorrect(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    // ── Alphanumeric (IN RFB nº 2.229/2024) ─────────────────────────────────

    [Theory]
    [InlineData("7Y.SLA.A4X/0001-57")]
    [InlineData("N1.H5P.GAX/0001-98")]
    [InlineData("3B.46L.6G8/0001-51")]
    [InlineData("ZL.YEK.4PN/0001-93")]
    [InlineData("6P.710.14N/0001-89")]
    public void Cnpj_ShouldBeValid_WhenAlphanumericCnpjIsValid(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.True(sut.IsAlphanumeric);
    }

    [Theory]
    [InlineData("S0.DH3.8BG/0001-95")]
    [InlineData("1C.6Y9.6AK/0001-11")]
    [InlineData("9W.J1B.VJY/0001-01")]
    [InlineData("HM.946.WPM/0001-81")]
    [InlineData("GE.Z70.B50/0001-88")]
    public void Cnpj_ShouldBeInvalid_WhenAlphanumericCnpjHasWrongCheckDigit(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("7YSLA A4X000157")]
    [InlineData("7Y.SLA.A4X/0001-5!")]
    public void Cnpj_ShouldBeInvalid_WhenAlphanumericCnpjHasInvalidCharacters(string number)
    {
        var sut = new Cnpj(number);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Cnpj_AlphanumericFormatted_ShouldReturnMaskedValue()
    {
        var sut = new Cnpj("7Y.SLA.A4X/0001-57");

        Assert.Equal("7Y.SLA.A4X/0001-57", sut.Formatted);
    }

    [Fact]
    public void Cnpj_AlphanumericRaw_ShouldNormalizeAndFormat()
    {
        var sut = new Cnpj("7YSLA A4X000157");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    // ── Shared behaviour ─────────────────────────────────────────────────────

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenNumericCnpjIsValid()
    {
        var result = Cnpj.TryParse("45723174000110", out var cnpj);

        Assert.True(result);
        Assert.True(cnpj.IsValid);
        Assert.Equal("45723174000110", cnpj.Number);
    }

    [Fact]
    public void TryParse_ShouldReturnTrue_WhenAlphanumericCnpjIsValid()
    {
        var result = Cnpj.TryParse("7Y.SLA.A4X/0001-57", out var cnpj);

        Assert.True(result);
        Assert.True(cnpj.IsValid);
        Assert.True(cnpj.IsAlphanumeric);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenCnpjIsInvalid()
    {
        var result = Cnpj.TryParse("12345678901234", out var cnpj);

        Assert.False(result);
        Assert.True(cnpj.IsInvalid);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidNumericCnpj()
    {
        Cnpj cnpj = "45723174000110";
        Assert.True(cnpj.IsValid);
        Assert.Equal("45723174000110", cnpj.Number);
    }

    [Fact]
    public void ImplicitOperator_ShouldCreateValidAlphanumericCnpj()
    {
        Cnpj cnpj = "7Y.SLA.A4X/0001-57";
        Assert.True(cnpj.IsValid);
        Assert.True(cnpj.IsAlphanumeric);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameNumericCnpj()
    {
        var a = new Cnpj("45723174000110");
        var b = new Cnpj("45.723.174/0001-10");

        Assert.True(a.Equals(b));
        Assert.True(a.Equals((object)b));
        Assert.Equal(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnTrue_ForSameAlphanumericCnpj()
    {
        var a = new Cnpj("7YSLAA4X000157");
        var b = new Cnpj("7Y.SLA.A4X/0001-57");

        Assert.Equal(a.Number, b.Number);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_ForDifferentCnpjNumbers()
    {
        var a = new Cnpj("45723174000110");
        var b = new Cnpj("12345678000199");

        Assert.False(a.Equals(b));
        Assert.NotEqual(a.GetHashCode(), b.GetHashCode());
    }

    [Fact]
    public void Formatted_ShouldReturnFormattedNumericCnpj()
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
