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
        var sut = new Email("contatoemailcom");
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
        var sut = new Email("contato@emailcom");
        Assert.Equal(string.Empty, sut.Domain);
        Assert.Equal(string.Empty, sut.TopLevelDomain);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenDomainAndTLDIsNotSeparatedWithDot()
    {
        var sut = new Email("contato@emailcom");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressHaveWhiteSpace()
    {
        var sut = new Email("contato@ email.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveAtSign()
    {
        var sut = new Email("contatoemail.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contato@@email.com")]
    [InlineData("contato@@@email.com")]
    public void Email_Should_BeInvalid_WhenItHasTwoOrMoreSigns(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveDomain()
    {
        var sut = new Email("contato@.com");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_Should_BeInvalid_WhenAddressDoesNotHaveTopLevelDomain()
    {
        var sut = new Email("contato@company");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Fact]
    public void Email_ShouldBeInvalid_WhenHostIsIncomplete()
    {
        var sut = new Email("contato@.com");
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
        var sut = new Email("contato@");
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contato@email..com")]
    [InlineData("contato@sub..domain.com")]
    public void Email_ShouldBeInvalid_WhenThereAreTwoDotsTogether(string address)
    {
        var sut = new Email(address);
        sut.Validate();

        Assert.True(sut.IsInvalid);
    }

    [Theory]
    [InlineData("contato@éxemplo.com")]
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
}