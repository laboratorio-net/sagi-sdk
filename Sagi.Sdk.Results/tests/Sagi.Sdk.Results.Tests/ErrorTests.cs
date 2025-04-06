namespace Sagi.Sdk.Results.Tests;

public class ErrorTests
{
    [Fact]
    public void Error_ShouldInitializeCorrectly()
    {
        var code = "TestCode";
        var message = "Test message";
        var error = new Error(code, message);

        Assert.Equal(code, error.Code);
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public void Error_WithNullCode_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => new Error(null!, "Test message"));
    }

    [Fact]
    public void Error_WithNullMessage_ShouldThrowArgumentException()
    {
        Assert.Throws<ArgumentNullException>(() => new Error("TestCode", null!));
    }
}