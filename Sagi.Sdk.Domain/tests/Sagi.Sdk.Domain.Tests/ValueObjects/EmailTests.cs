using Sagi.Sdk.Domain.ValueObjects;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Email_ShouldBeValid_WhenAddressIsValid()
    {
        var user = "contact";
        var domain = "fakerdomain";
        var topLevelDomain = "org.br";
        var host = $"{domain}.{topLevelDomain}";
        var address = $"{user}@{host}";

        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsValid);
        Assert.False(sut.IsInvalid);
        Assert.Equal(address, sut.Address);
        Assert.Equal(user, sut.User);
        Assert.Equal(host, sut.Host);
        Assert.Equal(domain, sut.Domain);
        Assert.Equal(topLevelDomain, sut.TopLevelDomain);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void Email_ShouldBeInvalid_WhenAddressIsNullorEmpty(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
        Assert.False(sut.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void HostAndUser_ShouldBeEmpty_WhenAdressIsNullOrEmpty(string address)
    {
        var sut = new Email(address);
        Assert.Equal(string.Empty, sut.Host);
        Assert.Equal(string.Empty, sut.User);
    }

    [Fact]
    public void HostAndUser_ShouldBeEmpty_WhenAdressHasNotSign()
    {
        var sut = new Email("contactemailcom");
        Assert.Equal(string.Empty, sut.Host);
        Assert.Equal(string.Empty, sut.User);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void DomainAndTopLevelDomain_ShouldBeEmpty_WhenHostIsNullOrEmpty(string address)
    {
        var sut = new Email(address);
        Assert.Equal(string.Empty, sut.Domain);
        Assert.Equal(string.Empty, sut.TopLevelDomain);
    }

    [Fact]
    public void DomainAndTopLevelDomain_ShouldBeEmpty_WhenHostHasNoDots()
    {
        var sut = new Email("contact@emailcom");
        Assert.Equal(string.Empty, sut.Domain);
        Assert.Equal(string.Empty, sut.TopLevelDomain);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenDomainAndTLDIsNotSeparatedWithDot()
    {
        var sut = new Email("contact@emailcom");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressHaveWhiteSpace()
    {
        var sut = new Email("contact@ email.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveAtSign()
    {
        var sut = new Email("contactemail.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contact@@email.com")]
    [InlineData("contact@@@email.com")]
    public void Email_Should_BeInvalid_WhenItHasTwoOrMoreSigns(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveDomain()
    {
        var sut = new Email("contact@.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveTopLevelDomain()
    {
        var sut = new Email("contact@company");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenHostIsIncomplete()
    {
        var sut = new Email("contact@.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenWithoutUser()
    {
        var sut = new Email("@email.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenWithoutHost()
    {
        var sut = new Email("contact@");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contact@email..com")]
    [InlineData("contact@sub..domain.com")]
    public void Email_ShouldBeInvalid_WhenThereAreTwoDotsTogether(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contact@éxemplo.com")]
    [InlineData("contâto@éxemplo.com")]
    [InlineData("contãto@éxemplo.com")]
    [InlineData("contto@exemplo.çom")]
    [InlineData("contatü@exemplo.com")]
    [InlineData("contàto@exemplo.com")]
    public void Email_ShouldBeInvalid_WhenItHasDiacritcs(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenAddressHasOverThanDefaultAllowedCharacteres()
    {
        var address = new string('a', 256) + "@email.com";
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenAddressHasOverThanSpecifiedCharacteres()
    {
        var maxLength = 120;
        var address = new string('a', 120) + "@email.com";
        var sut = new Email(address, maxLength);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void TryParse_ShouldReturnFalseAndNotParse_WhenValueIsInvalid()
    {
        var parsed = Email.TryParse("", out var email);
        Assert.False(parsed);
        Assert.True(email.Errors.Count > 0);
    }

    [Fact]
    public void TryParse_ShouldReturnTrueAndParse_WhenValueIsValid()
    {
        var expected = "contact@email.com";
        var parsed = Email.TryParse(expected, out var email);

        Assert.True(parsed);
        Assert.NotNull(email);
        Assert.Equal(expected, email.Address);
    }

    [Fact]
    public void ImplicitOperator_ShouldParse_StringToEmail()
    {
        var email = "contact@email.com";
        Email sut = email;

        Assert.Equal(email, sut.Address);
        Assert.True(sut.IsValid);
    }

    [Fact]
    public void ToString_ShouldReturn_EmailAddress()
    {
        var email = "contact@email.com";
        Email sut = email;

        Assert.Equal(email, sut.ToString());
    }

    [Fact]
    public void GetHashCode_ShouldBeEqual_ForEqualEmails()
    {
        var sut = new Email("contact@email.com");
        var other = new Email("contact@email.com");

        Assert.Equal(sut.GetHashCode(), other.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldBeDifferent_ForDifferentEmails()
    {
        var sut = new Email("contact@email.com");
        var other = new Email("other@email.com");

        Assert.NotEqual(sut.GetHashCode(), other.GetHashCode());
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenOtherIsNull()
    {
        var sut = new Email("contact@email.com");
        Email other = null;

        Assert.False(sut.Equals(other));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenReferences_AreTheSame()
    {
        var sut = new Email("contact@email.com");
        Email other = sut;

        Assert.True(sut.Equals(other));
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenValuesAreEquals()
    {
        var sut = new Email("contact@email.com");
        var other = new Email("contact@email.com");

        Assert.True(sut.Equals(other));
    }

    [Fact]
    public void EqualsObject_ShouldEvaluateEquality()
    {
        var sut = new Email("contact@email.com");
        var other = new Email("contact@email.com");

        Assert.True(sut.Equals((object)other));
    }
}