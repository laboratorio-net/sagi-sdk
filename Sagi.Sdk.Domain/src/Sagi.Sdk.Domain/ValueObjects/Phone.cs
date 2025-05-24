using System.Text.RegularExpressions;

using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public class Phone : ValueObject<Phone>
{
    public Phone(string ddi, string ddd, string number)
    {
        DDI = ddi;
        DDD = ddd;
        Number = number;
    }

    public string DDI { get; }
    public string DDD { get; }
    public string Number { get; }

    public string Formated
    {
        get
        {
            var number = long.Parse($"{DDD}{Number}")
                .ToString(@"(00) 0\.0000\-0000");

            return $"{DDI} {number}";
        }
    }

    public static short NumberMaxLength = 9;
    public static short DDDLength = 2;
    public static string DefaultDDI = "+55";
    public static int MaxLength => DefaultDDI.Length + DDDLength + NumberMaxLength;

    public override void Validate()
    {
        ClearErrors();

        const string error_code = "INVALID_PHONE";
        if (string.IsNullOrEmpty(Number))
        {
            AddError(new Error(error_code, "Phone number is required."));
        }

        if (!Number.All(char.IsDigit))
        {
            AddError(new Error(error_code, "Phone number must contain only digits."));
        }
    }

    public override string ToString() => Number;

    public static implicit operator Phone?(string value)
    {
        _ = TryParse(value, out var phone);
        return phone;
    }

    public static bool TryParse(string value, out Phone? phone)
    {
        ArgumentNullException.ThrowIfNull(value);

        value = OnlyDigits(value);
        if (value?.Length == MaxLength || value?.Length == MaxLength - 1)
        {
            var countryCode = $"+{value[..2]}";
            var ddd = value.Substring(2, 2);
            var number = value[4..];
            phone = new Phone(countryCode, ddd, number);
            phone.Validate();
            return phone.IsValid;
        }
        phone = default;
        return false;
    }

    public override bool Equals(Phone? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Number.Equals(other.Number, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => Equals(obj as Phone);

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Number.GetHashCode();
    }

    private static string OnlyDigits(string value)
    {
        var regex = new Regex(@"[^\d]", RegexOptions.Compiled);
        return string.IsNullOrWhiteSpace(value) ? string.Empty : regex.Replace(value, string.Empty);
    }
}