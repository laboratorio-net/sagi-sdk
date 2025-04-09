using Sagi.Sdk.Domain.Contracts;
using Sagi.Sdk.Domain.Entities;
using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Tests.Entities.Fake;

public class FakeEntity : Entity<Guid>
{
    public string? Subject { get; set; }
    public string? Message { get; set; }

    public void AddFakeError(IError error) => AddError(error);

    public void AddFakeErrors(List<IError> errors) => AddErrors(errors);

    public void LoadFakeEvent(Event<Guid> @event) => LoadEvent(@event);
    
    public void AddFakeEvent(Event<Guid> @event) => AddEvent(@event);

    protected override Guid GenerateId()
    {
        return Guid.NewGuid();
    }

    public FakeEntity Clone()
    {
        FakeEntity other = (FakeEntity)MemberwiseClone();
        return other;
    }

    public void Apply(FakeEvent @event)
    {
        Subject = @event.Subject;
        Message = @event.Message;
    }
}