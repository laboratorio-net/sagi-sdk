using System.Text.RegularExpressions;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed partial class Cnpj : ValueObject<Cnpj>
{
    [GeneratedRegex(@"^\d{14}$")]
    private static partial Regex NumericRawRegex();

    [GeneratedRegex(@"^[A-Z0-9]{14}$")]
    private static partial Regex AlphanumericRawRegex();

    [GeneratedRegex(@"^[A-Z0-9]{2}\.[A-Z0-9]{3}\.[A-Z0-9]{3}\/[A-Z0-9]{4}-\d{2}$")]
    private static partial Regex FormattedMaskRegex();

    [GeneratedRegex(@"[.\-/]")]
    private static partial Regex FormattedSeparatorsRegex();

    public Cnpj(string number)
    {
        Number = Normalize(number);
    }

    public string? Number { get; }

    public bool IsAlphanumeric => Number is not null && !NumericRawRegex().IsMatch(Number);

    public string Formatted
    {
        get
        {
            if (string.IsNullOrEmpty(Number) || Number.Length != 14)
                return string.Empty;

            if (!IsAlphanumeric && long.TryParse(Number, out long numeric))
                return numeric.ToString(@"00\.000\.000\/0000\-00");

            return $"{Number[..2]}.{Number[2..5]}.{Number[5..8]}/{Number[8..12]}-{Number[12..]}";
        }
    }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_CNPJ";

        if (string.IsNullOrWhiteSpace(Number))
        {
            AddError(new Error(errorCode, "CNPJ is required."));
            return;
        }

        if (Number.Length != 14)
        {
            AddError(new Error(errorCode, "CNPJ must contain exactly 14 characters."));
            return;
        }

        bool valid = IsAlphanumeric
            ? AlphanumericValidator.IsValid(Number)
            : NumericValidator.IsValid(Number);

        if (!valid)
            AddError(new Error(errorCode, "Invalid CNPJ number."));
    }

    public static bool TryParse(string number, out Cnpj cnpj)
    {
        cnpj = new Cnpj(number);
        cnpj.Validate();
        return cnpj.IsValid;
    }

    public static implicit operator Cnpj(string number)
    {
        _ = TryParse(number, out var cnpj);
        return cnpj;
    }

    public override bool Equals(Cnpj? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return string.Equals(Number, other.Number, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as Cnpj);

    public override int GetHashCode() => HashCode.Combine(Number?.ToUpperInvariant());

    public override string ToString() => Formatted;

    private static string? Normalize(string? input)
    {
        if (input is null)
            return null;

        string value = input.Trim().ToUpperInvariant();

        if (FormattedMaskRegex().IsMatch(value))
            return FormattedSeparatorsRegex().Replace(value, string.Empty);

        if (NumericRawRegex().IsMatch(value) || AlphanumericRawRegex().IsMatch(value))
            return value;

        string digitsOnly = new(value.Where(char.IsDigit).ToArray());
        if (digitsOnly.Length == 14)
            return digitsOnly;

        return value;
    }

    private static class NumericValidator
    {
        private static readonly int[] s_weights1 = [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];
        private static readonly int[] s_weights2 = [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        internal static bool IsValid(string cnpj)
        {
            if (IsAllSameDigit(cnpj)) return false;

            int first = CalculateDigit(cnpj[..12], s_weights1);
            int second = CalculateDigit(cnpj[..13], s_weights2);

            return cnpj[12] - '0' == first && cnpj[13] - '0' == second;
        }

        private static bool IsAllSameDigit(string cnpj)
        {
            char c = cnpj[0];
            for (int i = 1; i < cnpj.Length; i++)
                if (cnpj[i] != c) return false;
            return true;
        }

        private static int CalculateDigit(string slice, int[] weights)
        {
            int sum = 0;
            for (int i = 0; i < slice.Length; i++)
                sum += (slice[i] - '0') * weights[i];

            int remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }
    }

    private static class AlphanumericValidator
    {
        private const int AsciiOffset = 48;
        private const int Modulus = 11;
        private const int WeightMin = 2;
        private const int WeightMax = 9;

        internal static bool IsValid(string cnpj)
        {
            if (!HasValidFormat(cnpj)) return false;
            if (IsAllSameChar(cnpj)) return false;

            int first = CalculateCheckDigit(cnpj[..12]);
            int second = CalculateCheckDigit(cnpj[..13]);

            return cnpj[12] - '0' == first && cnpj[13] - '0' == second;
        }

        private static bool HasValidFormat(string cnpj)
        {
            for (int i = 0; i < 12; i++)
                if (!char.IsAsciiLetterUpper(cnpj[i]) && !char.IsAsciiDigit(cnpj[i]))
                    return false;

            return char.IsAsciiDigit(cnpj[12]) && char.IsAsciiDigit(cnpj[13]);
        }

        private static bool IsAllSameChar(string cnpj)
        {
            char first = cnpj[0];
            for (int i = 1; i < cnpj.Length; i++)
                if (cnpj[i] != first) return false;
            return true;
        }

        private static int CalculateCheckDigit(string baseChars)
        {
            int sum = 0;
            int weight = WeightMin;

            for (int i = baseChars.Length - 1; i >= 0; i--)
            {
                sum += (baseChars[i] - AsciiOffset) * weight;
                weight = weight == WeightMax ? WeightMin : weight + 1;
            }

            int remainder = sum % Modulus;
            return remainder < WeightMin ? 0 : Modulus - remainder;
        }
    }
}
