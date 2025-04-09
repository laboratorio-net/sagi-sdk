using Sagi.Sdk.Domain.Tests.Entities.Fake;
using Sagi.Sdk.Results;
using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Tests.Entities;

public class EntityTest
{
    [Fact]
    public void ShouldInitializeProperties()
    {
        var entity = new FakeEntity();

        Assert.NotEqual(Guid.Empty, entity.Id);
        Assert.True(entity.Active);
        Assert.Equal(DateTimeOffset.UtcNow.Date, entity.CreateAt.Date);
        Assert.Empty(entity.Errors);
        Assert.Empty(entity.Events);
    }

    [Fact]
    public void ShouldAddErrorToList()
    {
        var error = new Error("FAKE_ERROR", "Fake error message");
        var entity = new FakeEntity();

        entity.AddFakeError(error);

        Assert.Single(entity.Errors);
        Assert.Contains(error, entity.Errors);
    }

    [Fact]
    public void ShouldAddMultipleErrorsToList()
    {
        var errors = new List<IError>
        {
            new Error ("FAKE_ERROR_1", "Fake error message"),
            new Error ("FAKE_ERROR_2", "Fake error message"),
        };

        var entity = new FakeEntity();

        entity.AddFakeErrors(errors);

        Assert.Equal(2, entity.Errors.Count);
        Assert.Contains(errors[0], entity.Errors);
        Assert.Contains(errors[1], entity.Errors);
    }

    [Fact]
    public void ShouldReturnTrueIfErrorsExist()
    {
        var error = new Error("FAKE_ERROR", "Fake error message");
        var entity = new FakeEntity();

        entity.AddFakeError(error);

        Assert.True(entity.HasError());
    }

    [Fact]
    public void ShouldLoadEvent()
    {
        var @event = new FakeEvent();
        var entity = new FakeEntity();

        entity.LoadFakeEvent(@event);

        Assert.Equal(@event.Subject, entity.Subject);
        Assert.Equal(@event.Message, entity.Message);
        Assert.Equal(@event.AggregateVersion, entity.Version);
    }
    
    [Fact]
    public void ShouldAddEvent()
    {
        var entity = new FakeEntity();
        var @event = new FakeEvent();

        entity.AddFakeEvent(@event);
        
        Assert.Single(entity.Events); 
        Assert.Contains(@event, entity.Events);
    }

    [Fact]
    public void ShouldRemoveAllEvents()
    {
        var entity = new FakeEntity();
        var @event1 = new FakeEvent();
        var @event2 = new FakeEvent();

        entity.AddFakeEvent(@event1);
        entity.AddFakeEvent(@event2);
        entity.ClearEvents();

        Assert.Empty(entity.Events);
    }

    [Fact]
    public void ShouldReturnTrueForSameId()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1.Clone();

        Assert.True(entity1.Equals(entity2));
    }

    [Fact]
    public void ShouldReturnConsistentValue()
    {
        var entity = new FakeEntity();

        var hashCode1 = entity.GetHashCode();
        var hashCode2 = entity.GetHashCode();

        Assert.Equal(hashCode1, hashCode2);
    }
}