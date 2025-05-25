using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public class ZipCode : ValueObject<ZipCode>
{
    public static readonly short RequiredLength = 8;

    public ZipCode(string value)
    {
        ArgumentException.ThrowIfNullOrEmpty(value);
        Value = value;
        Validate();
    }

    public string Value { get; }
    public string Formatted
        => long.TryParse(Value, out var number) ?
           number.ToString(@"00\.000\-000") :
           string.Empty;

    public override bool Equals(ZipCode? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Value == other?.Value;
    }

    public override void Validate()
    {
        ClearErrors();
        const string error_code = "INVALID_PHONE";

        if (!Value.All(char.IsDigit))
            AddError(new Error(error_code, "Zipcode must contain only digits."));

        if (Value.Length != RequiredLength)
            AddError(new Error(error_code, $"Zipcode must have {RequiredLength} characters."));
    }

    public override string ToString() => Formatted;

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Value.GetHashCode();
    }

    public static bool TryParse(string value, out ZipCode? zipCode)
    {
        try
        {
            zipCode = new(value);

            if (zipCode.IsInvalid)
            {
                zipCode = null;
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            zipCode = null;
            return false;
        }
    }

    public static implicit operator ZipCode?(string value)
    {
        _ = TryParse(value, out var zipCode);
        return zipCode;
    }
}