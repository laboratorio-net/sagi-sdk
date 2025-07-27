using Sagi.Sdk.Domain.Contracts;

namespace Sagi.Sdk.Domain.Tests.Entities.Fake;

public class FakeEvent : Event<Guid>
{
    public FakeEvent()
    {
        SetAggregateVersion(1);
        SetAggregateId(Guid.NewGuid());
    }

    public override string Name => "FAKE_EVENT";
    public string Subject { get; set; } = "Fake subject";
    public string Message { get; set; } = "Fake message";
}