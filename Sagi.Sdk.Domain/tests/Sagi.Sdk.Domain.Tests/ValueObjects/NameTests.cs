using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class NameTests
{
    private static string FakeName => "João da Silva Sauro";

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Name_ShouldBeInvalid_WhenFirstNameIsNotInformed(string firstName)
    {
        var name = new Name(firstName, "Foo");
        Assert.True(name.IsInvalid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Name_ShouldBeInvalid_WhenLastNameIsNotInformed(string lastName)
    {
        var name = new Name("Foo", lastName);
        Assert.True(name.IsInvalid);
    }

    [Fact]
    public void FullName_ShouldReturn_AllParameters_EnteredToConstructor()
    {
        var name = new Name(FakeName.Split(" "));
        Assert.Equal(FakeName, name.FullName);
    }

    [Fact]
    public void Name_ShouldBeInvalid_WhenFullNameLength_IsLessThanMinLength()
    {
        var name = new Name(10, "Jo", "Sa");
        Assert.True(name.IsInvalid);
    }

    [Fact]
    public void Name_ShouldBeInvalid_WhenFullNameLenght_IsLessThanDefaultMinLenght()
    {
        var name = new Name("A", "S");
        Assert.True(name.IsInvalid);
    }

    [Fact]
    public void Name_ShouldBeInvalid_WhenFullNameLength_IsGreatherThanMaxLength()
    {
        var name = new Name(1, 4, "João", "Silva");
        Assert.True(name.IsInvalid);
    }

    [Fact]
    public void Name_ShouldBeInvalid_WhenFullNameLenght_IsGreatherThanDefaultMaxLenght()
    {
        var longName = new string('a', 100);
        var name = new Name(longName);
        Assert.True(name.IsInvalid);
    }

    [Fact]
    public void ToString_ShouldReturn_FullName()
    {
        var name = new Name(FakeName.Split(" "));
        Assert.Equal(FakeName, name.ToString());
    }

    [Fact]
    public void TryParse_ShouldBeTrue_WhenString_IsValidName()
    {
        var result = Name.TryParse(FakeName, out var name);

        Assert.True(result);
        Assert.Equal(FakeName, name.FullName);
    }

    [Fact]
    public void TryParse_ShouldBeFalse_WhenString_IsInvalidName()
    {
        var result = Name.TryParse("", out _);
        Assert.False(result);
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenStringIsNull()
    {
        Assert.Throws<ArgumentNullException>(
            () => Name.TryParse(null, out _));
    }

    [Fact]
    public void TryParse_ShouldReturnFalse_WhenStringIsWhitespace()
    {
        var result = Name.TryParse("   ", out _);
        Assert.False(result);
    }

    [Fact]
    public void ImplicitOperator_ShouldParse_StringToName()
    {
        Name name = FakeName;
        Assert.Equal(FakeName, name.FullName);
        Assert.True(name.IsValid);
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingSameInstance()
    {
        var name = new Name(FakeName.Split(" "));
        Assert.True(name.Equals(name));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenComparingEqualObjects()
    {
        var name1 = new Name(FakeName.Split(" "));
        var name2 = new Name(FakeName.Split(" "));
        Assert.True(name1.Equals(name2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingDifferentObjects()
    {
        var name1 = new Name("João", "Silva");
        var name2 = new Name("Maria", "Silva");
        Assert.False(name1.Equals(name2));
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenComparingWithNull()
    {
        var name = new Name(FakeName.Split(" "));
        Assert.False(name.Equals(null));
    }

    [Fact]
    public void GetHashCode_ShouldReturnSameValue_ForEqualObjects()
    {
        var name1 = new Name(FakeName.Split(" "));
        var name2 = new Name(FakeName.Split(" "));
        Assert.Equal(name1.GetHashCode(), name2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldReturnDifferentValues_ForDifferentObjects()
    {
        var name1 = new Name("João", "Silva");
        var name2 = new Name("Maria", "Silva");
        Assert.NotEqual(name1.GetHashCode(), name2.GetHashCode());
    }
}