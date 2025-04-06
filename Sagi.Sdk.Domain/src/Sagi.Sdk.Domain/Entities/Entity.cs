using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Entities;

public abstract class Entity<T> : IEquatable<Entity<T>>
{
    private readonly List<IError> _errors = [];

    protected Entity()
    {
        GenerateId();
        CreateAt = DateTimeOffset.UtcNow;
        Active = true;
    }

    public T Id { get; private set; } = default!;

    public bool Active { get; private set; }

    public DateTimeOffset CreateAt { get; private set; }

    public DateTimeOffset UpdateAt { get; protected set; }

    public IReadOnlyList<IError> Errors => [.. _errors];

    protected abstract void GenerateId();

    protected void AddError(IError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _errors.Add(error);
    }

    protected void AddErrors(IEnumerable<IError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));
        _errors.AddRange(errors);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Entity<T>);
    }

    public bool Equals(Entity<T>? other)
    {
        return other != null && EqualityComparer<T>.Default.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}