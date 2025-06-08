using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Country : ValueObject<Country>
{
    public Country(string name, string abbreviation, string isoCode)
    {
        Name = name?.Trim();
        Abbreviation = abbreviation?.Trim().ToUpperInvariant();
        IsoCode = isoCode?.Trim().ToUpperInvariant();
    }

    public string? Name { get; }
    public string? Abbreviation { get; }
    public string? IsoCode { get; }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_COUNTRY";

        if (string.IsNullOrWhiteSpace(Name))
        {
            AddError(new Error(errorCode, "Country name is required."));
        }

        if (string.IsNullOrWhiteSpace(Abbreviation) || Abbreviation.Length != 2)
        {
            AddError(new Error(errorCode,
                "Country abbreviation must be exactly 2 characters."));
        }

        if (string.IsNullOrWhiteSpace(IsoCode) || IsoCode.Length != 3)
        {
            AddError(new Error(errorCode,
                "Country ISO code must be exactly 3 characters."));
        }
    }

    public override bool Equals(Country? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               Abbreviation == other.Abbreviation &&
               IsoCode == other.IsoCode;
    }

    public override bool Equals(object? obj) => Equals(obj as Country);

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Abbreviation, IsoCode);
    }

    public override string ToString() => $"{Name} ({Abbreviation}/{IsoCode})";

    public static bool TryParse(
        string name,
        string abbreviation,
        string isoCode,
        out Country country)
    {
        country = new Country(name, abbreviation, isoCode);
        country.Validate();
        return country.IsValid;
    }

    public static implicit operator Country(
        (string Name, string Abbreviation, string IsoCode) tuple)
    {
        _ = TryParse(
            tuple.Name,
            tuple.Abbreviation,
            tuple.IsoCode,
            out var country);
            
        return country;
    }
}