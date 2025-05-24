using Sagi.Sdk.Domain.Contracts;

namespace Sagi.Sdk.Domain.ValueObjects;

public abstract class ValueObject<TChild> : Validateble, IEquatable<TChild>
{
    public abstract bool Equals(TChild? other);
}