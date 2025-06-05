
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class Neighborhood : ValueObject<Neighborhood>
{
    public Neighborhood(string name, City city)
    {
        Name = name?.Trim();
        City = city;
    }

    public string? Name { get; }
    public City City { get; }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_NEIGHBORHOOD";

        if (string.IsNullOrWhiteSpace(Name))
            AddError(new Error(errorCode, "Neighborhood name is required."));

        if (City is null || City.IsInvalid)
            AddError(new Error(errorCode, "A valid city must be provided."));
    }

    public override bool Equals(Neighborhood? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name && City.Equals(other.City);
    }

    public override bool Equals(object? obj) => Equals(obj as Neighborhood);

    public override int GetHashCode() => HashCode.Combine(Name, City);

    public override string ToString() => $"{Name} - {City.Name}/{City.State.Abbreviation}";

    public static bool TryParse(string name, City city, out Neighborhood neighborhood)
    {
        neighborhood = new Neighborhood(name, city);
        neighborhood.Validate();
        return neighborhood.IsValid;
    }

    public static implicit operator Neighborhood((string Name, City City) tuple)
    {
        _ = TryParse(tuple.Name, tuple.City, out var neighborhood);
        return neighborhood;
    }
}
