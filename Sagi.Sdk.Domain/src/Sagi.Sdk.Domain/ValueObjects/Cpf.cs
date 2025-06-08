
using System.Text.RegularExpressions;

using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Cpf : ValueObject<Cpf>
{
    public Cpf(string number)
    {
        var onlyNumbers = number?.Trim().Where(char.IsDigit) ?? "";
        Number = string.Join("", onlyNumbers);
    }

    public string? Number { get; }

    public string Formatted
        => long.TryParse(Number, out var number) ?
           number.ToString(@"000\.000\.000\-00") :
           string.Empty;

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_CPF";

        if (string.IsNullOrWhiteSpace(Number))
        {
            AddError(new Error(errorCode, "CPF is required."));
            return;
        }

        if (!Regex.IsMatch(Number, @"^\d{11}$"))
        {
            AddError(new Error(errorCode, "CPF must contain exactly 11 digits."));
            return;
        }

        if (!IsValidCpf(Number))
        {
            AddError(new Error(errorCode, "Invalid CPF number."));
        }
    }

    private static bool IsValidCpf(string cpf)
    {
        var invalids = new[]
        {
            "00000000000", "11111111111", "22222222222", "33333333333",
            "44444444444", "55555555555", "66666666666", "77777777777",
            "88888888888", "99999999999"
        };

        if (invalids.Contains(cpf)) return false;

        int[] mult1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] mult2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        var tempCpf = cpf[..9];
        var sum = 0;

        for (int i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * mult1[i];

        var remainder = sum % 11;
        var firstDigit = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += firstDigit;
        sum = 0;

        for (int i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * mult2[i];

        remainder = sum % 11;
        var secondDigit = remainder < 2 ? 0 : 11 - remainder;

        var expectedCpf = tempCpf + secondDigit;

        return cpf == expectedCpf;
    }

    public static bool TryParse(string number, out Cpf cpf)
    {
        cpf = new Cpf(number);
        cpf.Validate();
        return cpf.IsValid;
    }

    public static implicit operator Cpf(string number)
    {
        _ = TryParse(number, out var cpf);
        return cpf;
    }

    public override bool Equals(Cpf? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Number == other.Number;
    }

    public override bool Equals(object? obj) => Equals(obj as Cpf);

    public override int GetHashCode() => HashCode.Combine(Number);

    public override string ToString() => Formatted;
}
