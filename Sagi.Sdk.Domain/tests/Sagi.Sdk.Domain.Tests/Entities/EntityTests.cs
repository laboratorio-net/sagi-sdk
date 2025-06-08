using Sagi.Sdk.Domain.Tests.Entities.Fake;

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
        Assert.Empty(entity.Events);
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

    [Fact]
    public void ShouldReturnTrueForEqualEntitiesUsingEqualityOperator()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1;

        Assert.True(entity1 == entity2);
    }

    [Fact]
    public void ShouldReturnFalseForDifferentEntitiesUsingEqualityOperator()
    {
        var entity1 = new FakeEntity();
        var entity2 = new FakeEntity();

        Assert.False(entity1 == entity2);
    }

    [Fact]
    public void ShouldReturnFalseForNullAndEntityUsingEqualityOperator()
    {
        var entity = new FakeEntity();

        Assert.False(entity == null);
    }

    [Fact]
    public void ShouldReturnTrueForNullEntitiesUsingEqualityOperator()
    {
        FakeEntity? entity1 = null;
        FakeEntity? entity2 = null;

        Assert.True(entity1 == entity2);
    }

    [Fact]
    public void ShouldReturnTrueForDifferentEntitiesUsingInequalityOperator()
    {
        var entity1 = new FakeEntity();
        var entity2 = new FakeEntity();

        Assert.True(entity1 != entity2);
    }

    [Fact]
    public void ShouldReturnFalseForEqualEntitiesUsingInequalityOperator()
    {
        var entity1 = new FakeEntity();
        var entity2 = entity1;

        Assert.False(entity1 != entity2);
    }

    [Fact]
    public void ShouldReturnTrueForNullAndEntityUsingInequalityOperator()
    {
        var entity = new FakeEntity();

        Assert.True(entity != null);
    }

    [Fact]
    public void ShouldReturnFalseForNullEntitiesUsingInequalityOperator()
    {
        FakeEntity? entity1 = null;
        FakeEntity? entity2 = null;

        Assert.False(entity1 != entity2);
    }
}