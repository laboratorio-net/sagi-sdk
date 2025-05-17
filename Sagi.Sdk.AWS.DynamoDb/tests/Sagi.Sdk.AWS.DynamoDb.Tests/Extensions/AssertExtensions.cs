namespace Sagi.Sdk.AWS.DynamoDb.Tests.Extensions;

public class Asserts
{
    public static void Equal(DateTime expected, DateTime actual)
        => Equal(expected, actual, 1);

    public static void Equal(DateTime expected, DateTime actual, int tolerance)
    {
        var timespan = TimeSpan.FromMilliseconds(tolerance);
        Assert.True((expected - actual).Duration() < timespan,
        $"CreatedAt mismatch. Expected: {expected}, Actual: {actual}");
    }
}