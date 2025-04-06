namespace Sagi.Sdk.Results.Tests;

public class ResultTests
{
    [Fact]
    public void Result_WithSuccess_ShouldContainValue()
    {
        var value = "TestValue";
        var result = new Result<string>(value);


        Assert.True(result.IsSuccess);
        Assert.Equal(value, result.Value);
    }

    [Fact]
    public void Result_WithFailure_ShouldContainError()
    {
        var errors = new Error[] { new("TestError", "Test error message") };
        var result = new Result<string>(errors);

        Assert.True(result.IsFailure);
        Assert.Equal(errors, result.Errors);
    }

    [Fact]
    public void Result_WithNullError_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Result<string>((Error[])null!));
    }

    [Fact]
    public void Result_WithNullValue_ShouldThrowArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Result<string>(value: null!));
    }
}
