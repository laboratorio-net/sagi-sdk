using Sagi.Sdk.Domain.Contracts;
using Sagi.Sdk.Results;
using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Tests.Contracts;

public class ValidatebleTests
{
    [Fact]
    public void Validateble_ShouldBeInvalid_WhenHaveError()
    {
        Validateble sut = new FakeValidateble();

        sut.Validate();

        Assert.True(sut.IsInvalid);
        Assert.False(sut.IsValid);
    }

    [Fact]
    public void Validateble_ShouldBeValid_WhenHaveNoError()
    {
        Validateble sut = new FakeValidateble();

        Assert.True(sut.IsValid);
        Assert.False(sut.IsInvalid);
    }

    [Fact]
    public void ClearErrors_ShouldRemoveAllErrors()
    {
        Validateble sut = new FakeValidateble();

        sut.Validate();
        (sut as FakeValidateble)?.ClearFakeError();

        Assert.Empty(sut.Errors);
    }

    [Fact]
    public void ShouldAddErrorToList()
    {
        var error = new Error("FAKE_ERROR", "Fake error message");
        var sut = new FakeValidateble();

        sut.AddFakeError(error);

        Assert.Single(sut.Errors);
        Assert.Contains(error, sut.Errors);
    }

    [Fact]
    public void ShouldAddMultipleErrorsToList()
    {
        var errors = new List<IError>
        {
            new Error ("FAKE_ERROR_1", "Fake error message"),
            new Error ("FAKE_ERROR_2", "Fake error message"),
        };

        var sut = new FakeValidateble();

        sut.AddFakeErrors(errors);

        Assert.Equal(2, sut.Errors.Count);
        Assert.Contains(errors[0], sut.Errors);
        Assert.Contains(errors[1], sut.Errors);
    }
}

internal class FakeValidateble : Validateble
{
    public override void Validate() => AddError(new Error("FOO", "bar"));
    public void AddFakeError(IError error) => AddError(error);
    public void AddFakeErrors(IEnumerable<IError> errors) => AddErrors(errors);
    public void ClearFakeError() => ClearErrors();
}