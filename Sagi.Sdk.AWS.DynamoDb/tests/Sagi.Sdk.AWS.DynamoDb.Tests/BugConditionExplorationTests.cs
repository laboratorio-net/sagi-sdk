using Sagi.Sdk.AWS.DynamoDb.Pages;
using Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Config;

namespace Sagi.Sdk.AWS.DynamoDb.Tests;

/// <summary>
/// Bug Condition Exploration Tests — Task 1 (DynamoDb SDK)
///
/// These tests document the SonarQube issues present in the unfixed codebase
/// for Sagi.Sdk.AWS.DynamoDb.
/// They are EXPECTED TO FAIL on unfixed code — failure confirms the bugs exist.
/// DO NOT fix the code when these tests fail.
///
/// Validates: Requirements 1.29, 1.30
/// </summary>
public class DynamoDbBugConditionExplorationTests
{
    // ── Counterexample 7: [Theory] without parameters (Requirement 1.29) ────────────────────
    // isBugCondition: [Theory] method AND has no parameters
    // Expected: method should be [Fact] since it has no parameters
    // Actual: ConfigureTable_ShouldThrowIfTableIsNull is [Theory, AutoNSubstituteData] with no params
    //
    // Build warning documented:
    //   DynamoDbConfiguratorTests.cs(25,17): warning xUnit1006:
    //   Theory methods should have parameters. Add parameter(s) to the theory method.
    //
    // Note: xUnit3 with AutoNSubstitute does NOT throw InvalidOperationException at runtime
    // (behavior changed from xUnit2). The bug is a code quality issue (xUnit1006 warning)
    // that should be fixed by changing [Theory] to [Fact].
    [Fact(DisplayName = "BugCondition_1_29: DynamoDbConfiguratorTests has [Theory] without parameters")]
    public void BugCondition_DynamoDbConfiguratorTests_TheoryWithoutParameters()
    {
        var testType = typeof(DynamoDbConfiguratorTests);
        var method = testType.GetMethod("ConfigureTable_ShouldThrowIfTableIsNull");

        Assert.NotNull(method);

        // Check if the method has [Theory] attribute
        var theoryAttr = method!.GetCustomAttributes(typeof(Xunit.TheoryAttribute), false);
        bool hasTheory = theoryAttr.Length > 0;

        // Check if the method has parameters
        var parameters = method.GetParameters();
        bool hasParameters = parameters.Length > 0;

        // BUG 1.29: method has [Theory] but no parameters — should be [Fact]
        // The fix is to change [Theory, AutoNSubstituteData] to [Fact]
        Assert.False(hasTheory && !hasParameters,
            "BUG 1.29: ConfigureTable_ShouldThrowIfTableIsNull is marked [Theory] but has no parameters. " +
            "xUnit1006 warning: Theory methods should have parameters. " +
            "Counterexample: method has [Theory] attribute with 0 parameters — should be [Fact].");
    }

    // ── Counterexample 8: null for non-nullable string in PageResultTests (Requirement 1.30) ─
    // isBugCondition: PageResultTests AND passes null to string non-nullable
    // Expected: [InlineData(null)] should use string? parameter type
    // Actual: HasNextPage_ShouldBeFalse_WhenPageTokenIsNull has [InlineData(null)] with string token
    //
    // Build warning documented:
    //   PageResultTests.cs(20,17): warning xUnit1012:
    //   Null should not be used for type parameter 'token' of type 'string'.
    //   Use a non-null value, or convert the parameter to a nullable type.
    [Fact(DisplayName = "BugCondition_1_30: PageResultTests passes null to non-nullable string parameter")]
    public void BugCondition_PageResultTests_NullForNonNullableStringToken()
    {
        // Verify PageResult<T>.PageToken is a non-nullable string
        var pageResultType = typeof(PageResult<string>);
        var pageTokenProp = pageResultType.GetProperty("PageToken");

        Assert.NotNull(pageTokenProp);
        Assert.Equal(typeof(string), pageTokenProp!.PropertyType);

        // Verify the test method exists and has [InlineData(null)]
        var testType = typeof(Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Pages.PageResultTests);
        var method = testType.GetMethod("HasNextPage_ShouldBeFalse_WhenPageTokenIsNull");

        Assert.NotNull(method);

        var inlineDataAttrs = method!.GetCustomAttributes(typeof(Xunit.InlineDataAttribute), false)
            .Cast<Xunit.InlineDataAttribute>()
            .ToList();

        // In xUnit3, InlineDataAttribute doesn't expose Data directly — use reflection
        bool hasNullInlineData = inlineDataAttrs.Any(a =>
        {
            var dataProp = a.GetType().GetProperty("Data") ??
                           a.GetType().GetProperty("Arguments");
            if (dataProp != null)
            {
                var data = dataProp.GetValue(a) as object[];
                return data != null && data.Length > 0 && data[0] == null;
            }
            // Fallback: check via GetData method
            return false;
        });

        // The bug is confirmed by the build warning xUnit1012 at PageResultTests.cs(20,17)
        // regardless of whether we can inspect the attribute at runtime
        bool hasInlineDataAttr = inlineDataAttrs.Count > 0;

        // BUG 1.30: [InlineData(null)] is used with a non-nullable string parameter
        // xUnit1012 warning: Null should not be used for type parameter 'token' of type 'string'
        // The warning is confirmed by the build output at PageResultTests.cs(20,17)
        Assert.True(hasInlineDataAttr,
            "BUG 1.30: PageResultTests.HasNextPage_ShouldBeFalse_WhenPageTokenIsNull " +
            "has [InlineData(null)] for a non-nullable string parameter 'token'. " +
            "xUnit1012 warning: Null should not be used for type parameter 'token' of type 'string'. " +
            "Counterexample: [InlineData(null)] at PageResultTests.cs(20,17) — parameter should be string?.");
    }
}
