using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public class Name : ValueObject, IEquatable<Name>
{
    public Name(params string[] names)
    {
        FirstName = names[0];
        LastName = names[^1];
        FullName = names.Aggregate((agg, current) => $"{agg} {current}");
        MinLength = 4;
        MaxLength = 80;
        Validate();
    }

    public Name(short minLength, params string[] names)
        : this(names)
    {
        MinLength = minLength;
        Validate();
    }

    public Name(short minLength, short maxLength, params string[] names)
        : this(minLength, names)
    {
        MaxLength = maxLength;
        Validate();
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string FullName { get; }
    public short MinLength { get; }
    public short MaxLength { get; }

    public override void Validate()
    {
        ClearErrors();

        if (string.IsNullOrEmpty(FirstName))
        {
            AddError(new Error("INVALID_NAME", "Firstname is required."));
        }

        if (string.IsNullOrEmpty(LastName))
        {
            AddError(new Error("INVALID_NAME", "Last is required."));
        }

        if (FullName.Length < MinLength)
        {
            AddError(new Error("INVALID_NAME",
                $"FullName must have at least {MinLength} characters."));
        }

        if (FullName.Length > MaxLength)
        {
            AddError(new Error("INVALID_NAME",
                $"FullName must have maximum of {MaxLength} characters."));
        }
    }

    public override string ToString() => FullName;

    public static implicit operator Name(string value)
    {
        _ = TryParse(value, out var name);
        return name;
    }

    public static bool TryParse(string value, out Name name)
    {
        ArgumentNullException.ThrowIfNull(value);

        name = new Name(value.Split(" "));
        return name.IsValid;
    }

    public bool Equals(Name? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return FullName.Equals(other.FullName, StringComparison.Ordinal);
    }

    public override bool Equals(object obj) => Equals(obj as Name);

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + FullName.GetHashCode();
    }
}