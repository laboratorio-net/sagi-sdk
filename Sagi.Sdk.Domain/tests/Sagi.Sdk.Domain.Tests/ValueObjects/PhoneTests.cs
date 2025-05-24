using Sagi.Sdk.Domain.ValueObjects;


namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class PhoneTests
{
    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("00123456789")] // DDI inválido
    public void TryParse_ShouldReturnFalse_WhenInputIsInvalid(string input)
    {
        var result = Phone.TryParse(input, out var phone);
        Assert.False(result);
        Assert.Null(phone);
    }

    [Fact]
    public void TryParse_ShouldThrowException_WhenInputIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => Phone.TryParse(null, out _));
    }

    [Theory]
    [InlineData("5511987654321", "+55", "11", "987654321")] // 9 dígitos
    [InlineData("551112345678", "+55", "11", "12345678")]   // 8 dígitos
    public void TryParse_ShouldReturnPhone_WhenInputIsValid(string input, string expectedDdi, string expectedDdd, string expectedNumber)
    {
        var result = Phone.TryParse(input, out var phone);
        Assert.True(result);
        Assert.NotNull(phone);
        Assert.Equal(expectedDdi, phone.DDI);
        Assert.Equal(expectedDdd, phone.DDD);
        Assert.Equal(expectedNumber, phone.Number);
    }

    [Theory]
    [InlineData("+55", "11", "987654321", "+55 (11) 9 8765-4321")]
    [InlineData("+55", "84", "12345678", "+55 (84) 1234-5678")]
    public void Formatted_ShouldReturnFormattedPhone(string ddi, string ddd, string number, string expectedFormatted)
    {
        var phone = new Phone(ddi, ddd, number);
        Assert.Equal(expectedFormatted, phone.Formatted);
    }

    [Fact]
    public void ToString_ShouldReturnFormattedPhone()
    {
        var phone = new Phone("+55", "84", "987654321");
        Assert.Equal(phone.Formatted, phone.ToString());
    }

    [Fact]
    public void Validate_ShouldAddError_WhenNumberIsInvalid()
    {
        var phone = new Phone("+55", "84", "98A76"); // contém letra
        phone.Validate();
        Assert.True(phone.IsInvalid);
    }

    [Fact]
    public void Validate_ShouldAddError_WhenDDDIsInvalid()
    {
        var phone = new Phone("+55", "8", "987654321"); // DDD com 1 dígito
        phone.Validate();
        Assert.True(phone.IsInvalid);
    }

    [Fact]
    public void Validate_ShouldAddError_WhenDDIIsInvalid()
    {
        var phone = new Phone("55", "84", "987654321"); // falta o '+'
        phone.Validate();
        Assert.True(phone.IsInvalid);
    }

    [Fact]
    public void ImplicitConversion_ShouldReturnValidPhone_WhenInputIsValid()
    {
        Phone phone = "5511987654321";
        Assert.NotNull(phone);
        Assert.Equal("+55", phone.DDI);
        Assert.Equal("11", phone.DDD);
        Assert.Equal("987654321", phone.Number);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenPhonesAreEqual()
    {
        var phone1 = new Phone("+55", "84", "12345678");
        var phone2 = new Phone("+55", "84", "12345678");

        Assert.True(phone1.Equals(phone2));
        Assert.True(phone1.Equals((object)phone2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenPhonesAreDifferent()
    {
        var phone1 = new Phone("+55", "84", "12345678");
        var phone2 = new Phone("+55", "84", "87654321");

        Assert.False(phone1.Equals(phone2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherIsNull()
    {
        var phone = new Phone("+55", "84", "12345678");
        Assert.False(phone.Equals(null));
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_ForEqualPhones()
    {
        var phone1 = new Phone("+55", "84", "12345678");
        var phone2 = new Phone("+55", "84", "12345678");

        Assert.Equal(phone1.GetHashCode(), phone2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferent_ForDifferentPhones()
    {
        var phone1 = new Phone("+55", "84", "12345678");
        var phone2 = new Phone("+55", "84", "87654321");

        Assert.NotEqual(phone1.GetHashCode(), phone2.GetHashCode());
    }
}