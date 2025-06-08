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
        : this(
              street,
              number,
              complement,
              neighborhood,
              zipCode,
              streetMinLength: 2,
              streetMaxLength: 80,
              numberMinLength: 1,
              numberMaxLength: 10)
    { }

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

        StreetMinLength = streetMinLength;
        StreetMaxLength = streetMaxLength;
        NumberMinLength = numberMinLength;
        NumberMaxLength = numberMaxLength;
    }

    public string? Street { get; }
    public string? Number { get; }
    public string? Complement { get; }
    public Neighborhood Neighborhood { get; }
    public ZipCode ZipCode { get; }

    public short StreetMinLength { get; }
    public short StreetMaxLength { get; }
    public short NumberMinLength { get; }
    public short NumberMaxLength { get; }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_ADDRESS";

        if (string.IsNullOrWhiteSpace(Street) ||
            Street.Length < StreetMinLength ||
            Street.Length > StreetMaxLength)
        {
            AddError(new Error(errorCode,
            $"Street must be between {StreetMinLength} and {StreetMinLength} characters."));
        }

        if (string.IsNullOrWhiteSpace(Number) ||
            Number.Length < NumberMinLength ||
            Number.Length > NumberMaxLength)
        {
            AddError(new Error(errorCode,
            $"Number must be between {NumberMinLength} and {NumberMaxLength} characters."));
        }

        Validate(Neighborhood, errorCode, "A valid neighborhood must be provided.");
        Validate(ZipCode, errorCode, "A valid ZIP code must be provided.");
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

    public override string ToString() =>
        $"{Street}, {Number} - {Neighborhood.Name}, {Neighborhood.City.Name} " +
        $"- {Neighborhood.City.State.Abbreviation}/" +
        $"{Neighborhood.City.State.Country.Abbreviation} - ZIP: {ZipCode}";
}