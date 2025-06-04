using Sagi.Sdk.Domain.Contracts;

namespace Sagi.Sdk.Domain.ValueObjects;

public abstract class ValueObject<TChild> : Validateble, IEquatable<TChild>
{
    public abstract bool Equals(TChild? other);
    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();
    public abstract override string ToString();
}