using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public sealed class State : ValueObject<State>
{
    public State(string name, string abbreviation, Country country)
    {
        Name = name?.Trim();
        Abbreviation = abbreviation?.Trim().ToUpperInvariant();
        Country = country;
    }

    public string Name { get; }
    public string Abbreviation { get; }
    public Country Country { get; }

    public override void Validate()
    {
        ClearErrors();
        const string errorCode = "INVALID_STATE";

        if (string.IsNullOrWhiteSpace(Name))
            AddError(new Error(errorCode, "State name is required."));

        if (string.IsNullOrWhiteSpace(Abbreviation) || Abbreviation.Length != 2)
            AddError(new Error(errorCode, "State abbreviation must be exactly 2 characters."));

        if (Country is null || Country.IsInvalid)
            AddError(new Error(errorCode, "A valid country must be provided."));
    }

    public override bool Equals(State? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Name == other.Name &&
               Abbreviation == other.Abbreviation &&
               Country.Equals(other.Country);
    }

    public override bool Equals(object? obj) => Equals(obj as State);

    public override int GetHashCode()
    {
        return HashCode.Combine(Name, Abbreviation, Country);
    }

    public override string ToString() => $"{Name} ({Abbreviation}) - {Country.Abbreviation}";

    public static bool TryParse(string name, string abbreviation, Country country, out State state)
    {
        state = new State(name, abbreviation, country);
        state.Validate();
        return state.IsValid;
    }

    public static implicit operator State((string Name, string Abbreviation, Country Country) tuple)
    {
        _ = TryParse(tuple.Name, tuple.Abbreviation, tuple.Country, out var state);
        return state;
    }
} 