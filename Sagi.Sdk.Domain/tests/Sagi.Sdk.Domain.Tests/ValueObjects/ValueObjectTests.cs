using Sagi.Sdk.Domain.ValueObjects;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.Tests.ValueObjects;

public class ValueObjectTests
{
    [Fact]
    public void ValueObject_ShouldBeInvalid_WhenHaveError()
    {
        ValueObject vo = new FakeValueObject();

        vo.Validate();

        Assert.True(vo.IsInvalid);
        Assert.False(vo.IsValid);
    }

    [Fact]
    public void ValueObject_ShouldBeValid_WhenHaveNoError()
    {
        ValueObject vo = new FakeValueObject();

        Assert.True(vo.IsValid);
        Assert.False(vo.IsInvalid);
    }

    [Fact]
    public void ClearErrors_ShouldRemoveAllErrors()
    {
        ValueObject vo = new FakeValueObject();

        vo.Validate();
        (vo as FakeValueObject)?.Clear();

        Assert.Empty(vo.Errors);
    }
}


internal class FakeValueObject : ValueObject
{
    public override void Validate() => AddError(new Error("FOO", "bar"));
    public void Clear() => ClearErrors();
}