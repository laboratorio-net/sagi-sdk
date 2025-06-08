using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class City : ValueObject<City>
{
    public City(string name, State state)
    {
        Name = name?.Trim();
        State = state;
    }

    public string? Name { get; }
    public State State { get; }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_CITY";

        if (string.IsNullOrWhiteSpace(Name))
            AddError(new Error(errorCode, "City name is required."));
            
        Validate(State, errorCode, "A valid state must be provided.");
    }

    public override bool Equals(City? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name && State.Equals(other.State);
    }

    public override bool Equals(object? obj) => Equals(obj as City);

    public override int GetHashCode() => HashCode.Combine(Name, State);

    public override string ToString() => $"{Name} - {State.Abbreviation}/{State.Country.Abbreviation}";

    public static bool TryParse(string name, State state, out City city)
    {
        city = new City(name, state);
        city.Validate();
        return city.IsValid;
    }

    public static implicit operator City((string Name, State State) tuple)
    {
        _ = TryParse(tuple.Name, tuple.State, out var city);
        return city;
    }
}