using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Address : ValueObject<Address>
{
    public Address(
        string street,
        string number,
        string complement,
        Neighborhood neighborhood,
        ZipCode zipCode)
        : this(street, number, complement, neighborhood, zipCode, StreetMinLength, StreetMaxLength, NumberMinLength, NumberMaxLength)
    {
    }

    public Address(
        string street,
        string number,
        string complement,
        Neighborhood neighborhood,
        ZipCode zipCode,
        short streetMinLength,
        short streetMaxLength,
        short numberMinLength,
        short numberMaxLength)
    {
        Street = street?.Trim();
        Number = number?.Trim();
        Complement = complement?.Trim();
        Neighborhood = neighborhood;
        ZipCode = zipCode;

        StreetMin = streetMinLength;
        StreetMax = streetMaxLength;
        NumberMin = numberMinLength;
        NumberMax = numberMaxLength;
    }

    public string Street { get; }
    public string Number { get; }
    public string Complement { get; }
    public Neighborhood Neighborhood { get; }
    public ZipCode ZipCode { get; }

    public short StreetMin { get; }
    public short StreetMax { get; }
    public short NumberMin { get; }
    public short NumberMax { get; }

    public static short StreetMinLength => 2;
    public static short StreetMaxLength => 80;
    public static short NumberMinLength => 1;
    public static short NumberMaxLength => 10;

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_ADDRESS";

        if (string.IsNullOrWhiteSpace(Street) || Street.Length < StreetMin || Street.Length > StreetMax)
            AddError(new Error(errorCode, $"Street must be between {StreetMin} and {StreetMax} characters."));

        if (string.IsNullOrWhiteSpace(Number) || Number.Length < NumberMin || Number.Length > NumberMax)
            AddError(new Error(errorCode, $"Number must be between {NumberMin} and {NumberMax} characters."));

        if (Neighborhood is null || Neighborhood.IsInvalid)
            AddError(new Error(errorCode, "A valid neighborhood must be provided."));

        if (ZipCode is null || ZipCode.IsInvalid)
            AddError(new Error(errorCode, "A valid ZIP code must be provided."));
    }

    public override bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Street == other.Street &&
               Number == other.Number &&
               Complement == other.Complement &&
               Neighborhood.Equals(other.Neighborhood) &&
               ZipCode.Equals(other.ZipCode);
    }

    public override bool Equals(object? obj) => Equals(obj as Address);

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, Number, Complement, Neighborhood, ZipCode);
    }

    public override string ToString()
    {
        return $"{Street}, {Number} - {Neighborhood.Name}, {Neighborhood.City.Name} - {Neighborhood.City.State.Abbreviation}/{Neighborhood.City.State.Country.Abbreviation} - ZIP: {ZipCode}";
    }
}
