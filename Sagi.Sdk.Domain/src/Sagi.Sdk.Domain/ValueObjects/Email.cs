using System.Text.RegularExpressions;

using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Email : ValueObject<Email>
{
    public Email(string address) : this(address, 256) { }

    public Email(string address, int maxLength)
    {
        Address = string.IsNullOrWhiteSpace(address)
            ? address
            : address.Trim().ToLower();

        Host = GetHost();
        User = GetUser();
        Domain = GetDomain();
        TopLevelDomain = GetTopLevenDomain();
        MaxLength = maxLength;
    }

    private string GetHost()
    {
        if (string.IsNullOrEmpty(Address))
        {
            return string.Empty;
        }

        var data = Address.Split('@');
        return data?.Length == 2 ? data[1] : string.Empty;
    }

    private string GetUser()
    {
        if (string.IsNullOrEmpty(Address))
        {
            return string.Empty;
        }

        var data = Address.Split('@');
        return data?.Length == 2 ? data[0] : string.Empty;
    }

    private string GetDomain()
    {
        if (string.IsNullOrEmpty(Host))
        {
            return string.Empty;
        }

        var data = Host.Split('.');
        return data?.Length > 1 ? data[0] : string.Empty;
    }

    private string GetTopLevenDomain()
    {
        if (string.IsNullOrEmpty(Host))
        {
            return string.Empty;
        }

        var data = Host.Split('.');
        return data?.Length > 1 ? string.Join('.', data[1..]) : string.Empty;
    }

    public string Address { get; }

    public string User { get; }

    public string Host { get; }

    public string Domain { get; }

    public string TopLevelDomain { get; }

    public int MaxLength { get; }

    public override void Validate()
    {
        ClearErrors();
        const string error_code = "INVALID_EMAIL";

        if (string.IsNullOrWhiteSpace(Address))
        {
            AddError(new Error(error_code, "E-mail address is required."));
        }

        if (!string.IsNullOrEmpty(Address) && Address.Length > MaxLength)
        {
            AddError(new Error(error_code, "Email address is too long."));
        }

        ValidateAddressPattern(error_code);
    }

    private void ValidateAddressPattern(string errorCode)
    {
        try
        {
            var pattern = @"^[a-zA-Z0-9_.-]+@([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}$";
            var isMatch = Regex.IsMatch(Address, pattern, RegexOptions.IgnoreCase);
            if (isMatch is false)
                AddError(new Error(errorCode, "The entered e-mail is invalid."));
        }
        catch (Exception)
        {
            AddError(new Error(errorCode, "The entered e-mail is invalid."));
        }
    }

    public static implicit operator Email(string value)
    {
        _ = TryParse(value, out var email);
        return email;
    }

    public static bool TryParse(string address, out Email email)
    {
        email = new Email(address);
        email.Validate();
        return email.IsValid;
    }

    public override bool Equals(Email? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Address == other.Address;
    }

    public override bool Equals(object? obj) =>
        Equals(obj as Email);

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Address.GetHashCode();
    }

    public override string ToString() => Address;
}