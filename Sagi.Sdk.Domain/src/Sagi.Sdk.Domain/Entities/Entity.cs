using Sagi.Sdk.Domain.Contracts;
using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Entities;

public abstract class Entity<T> : Validateble
{
    private readonly List<IError> _errors = [];
    private readonly List<Event<T>> _events = [];

    protected Entity()
    {
        Id = GenerateId();
        CreateAt = DateTimeOffset.UtcNow;
        Active = true;
    }

    public T Id { get; private set; } = default!;

    public bool Active { get; private set; }

    public DateTimeOffset CreateAt { get; private set; }

    public DateTimeOffset UpdateAt { get; protected set; }

    public long Version { get; protected set; }


    public IReadOnlyList<Event<T>> Events => [.. _events.AsReadOnly()];

    protected abstract T GenerateId();

    protected void UpVersion() => Version += 1;

    public void ClearEvents() => _events.Clear();

    protected void LoadEvent(Event<T> @event)
    {
        ((dynamic)this).Load((dynamic)@event);
        Version = @event.AggregateVersion;
    }

    protected void AddEvent<TEvent>(TEvent @event) where TEvent : Event<T>
    {
        UpVersion();
        @event.SetAggregateVersion(Version);
        @event.SetAggregateId(Id);
        _events.Add(@event);
        LoadEvent(@event);
    }

    public override bool Equals(object? obj)
    {
        var compareTo = obj as Entity<T>;

        if (ReferenceEquals(this, compareTo)) return true;
        if (ReferenceEquals(null, compareTo)) return false;

        return Id!.Equals(compareTo.Id);
    }

    public static bool operator ==(Entity<T> a, Entity<T> b)
    {
        if (ReferenceEquals(a, null) && ReferenceEquals(b, null))
            return true;

        if (ReferenceEquals(a, null) || ReferenceEquals(b, null))
            return false;

        return a.Equals(b);
    }

    public static bool operator !=(Entity<T> a, Entity<T> b)
    {
        return !(a == b);
    }

    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Id!.GetHashCode();
    }
}