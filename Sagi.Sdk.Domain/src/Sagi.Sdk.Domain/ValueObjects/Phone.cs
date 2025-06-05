using System.Text.RegularExpressions;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Phone : ValueObject<Phone>, IEquatable<Phone>
{
    public static readonly short NumberMaxLength = 9;
    public static readonly short DDDLength = 2;
    public static readonly string DefaultDDI = "+55";
    public static int MaxLength => DefaultDDI.Length + DDDLength + NumberMaxLength;

    public string DDI { get; }
    public string DDD { get; }
    public string Number { get; }

    public Phone(string ddi, string ddd, string number)
    {
        DDI = ddi;
        DDD = ddd;
        Number = number;
    }

    public string Formatted
    {
        get
        {
            if (Number.Length == 9)
                return $"{DDI} ({DDD}) {Number[0]} {Number.Substring(1, 4)}-{Number.Substring(5)}";

            return $"{DDI} ({DDD}) {Number.Substring(0, 4)}-{Number.Substring(4)}";
        }
    }

    public override void Validate()
    {
        ClearErrors();
        const string error_code = "INVALID_PHONE";

        if (string.IsNullOrWhiteSpace(Number) || !Number.All(char.IsDigit))
            AddError(new Error(error_code, "Phone number must contain only digits."));

        if (string.IsNullOrWhiteSpace(DDD) || DDD.Length != DDDLength || !DDD.All(char.IsDigit))
            AddError(new Error(error_code, "DDD must contain exactly two digits."));

        if (string.IsNullOrWhiteSpace(DDI) || !Regex.IsMatch(DDI, @"^\+\d{1,4}$"))
            AddError(new Error(error_code, "DDI must start with '+' and contain up to 4 digits."));
    }

    public override string ToString() => Formatted;

    public static implicit operator Phone?(string value)
    {
        _ = TryParse(value, out var phone);
        return phone;
    }

    public static bool TryParse(string value, out Phone? phone)
    {
        ArgumentNullException.ThrowIfNull(value);

        value = OnlyDigits(value);
        var match = Regex.Match(value, @"^(?<ddi>\d{2})(?<ddd>\d{2})(?<number>\d{8,9})$");
        if (match.Success)
        {
            var ddi = $"+{match.Groups["ddi"].Value}";
            var ddd = match.Groups["ddd"].Value;
            var number = match.Groups["number"].Value;

            phone = new Phone(ddi, ddd, number);
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

        return DDI == other.DDI && DDD == other.DDD && Number == other.Number;
    }

    public override bool Equals(object? obj) => Equals(obj as Phone);

    public override int GetHashCode() => HashCode.Combine(DDI, DDD, Number);

    private static string OnlyDigits(string value)
    {
        var regex = new Regex(@"[^\d]", RegexOptions.Compiled);
        return string.IsNullOrWhiteSpace(value) ? string.Empty : regex.Replace(value, string.Empty);
    }
}