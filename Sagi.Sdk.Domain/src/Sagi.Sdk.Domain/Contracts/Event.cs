namespace Sagi.Sdk.Domain.Contracts;

public abstract class Event<T>
{
    public T? AggregateId { get; protected set; }

    public long AggregateVersion { get; protected set; }

    public DateTimeOffset Timestamp { get; protected set; }

    public abstract string Name { get; }

    public Event()
    {
        Timestamp = DateTimeOffset.UtcNow;
    }

    public void SetAggregateId(T aggregateId)
    {
        ArgumentNullException.ThrowIfNull(aggregateId);
        AggregateId = aggregateId;
    }

    public void SetAggregateVersion(long aggregateVersion)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(aggregateVersion, 0);
        AggregateVersion = aggregateVersion;
    }
}