using System.Text.RegularExpressions;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Cnpj : ValueObject<Cnpj>
{
    public Cnpj(string number)
    {
        var onlyNumbers = number?.Trim().Where(char.IsDigit) ?? "";
        Number = string.Join("", onlyNumbers);
    }

    public string? Number { get; }

    public string Formatted
        => long.TryParse(Number, out var number) ?
           number.ToString(@"00\.000\.000\/0000\-00") :
           string.Empty;

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_CNPJ";

        if (string.IsNullOrWhiteSpace(Number))
        {
            AddError(new Error(errorCode, "CNPJ is required."));
            return;
        }

        if (!Regex.IsMatch(Number, @"^\d{14}$"))
        {
            AddError(new Error(errorCode, "CNPJ must contain exactly 14 digits."));
            return;
        }

        if (!IsValidCnpj(Number))
        {
            AddError(new Error(errorCode, "Invalid CNPJ number."));
        }
    }

    private static bool IsValidCnpj(string cnpj)
    {
        var invalids = new[]
        {
            "00000000000000", "11111111111111", "22222222222222", "33333333333333",
            "44444444444444", "55555555555555", "66666666666666", "77777777777777",
            "88888888888888", "99999999999999"
        };

        if (invalids.Contains(cnpj)) return false;

        int[] mult1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCnpj = cnpj[..12];
        var sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * mult1[i];

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        tempCnpj += firstDigit;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * mult2[i];

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        var expectedCnpj = tempCnpj + secondDigit;

        return cnpj == expectedCnpj;
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

        return Number == other.Number;
    }

    public override bool Equals(object? obj) => Equals(obj as Cnpj);

    public override int GetHashCode() => HashCode.Combine(Number);

    public override string ToString() => Formatted;
}
