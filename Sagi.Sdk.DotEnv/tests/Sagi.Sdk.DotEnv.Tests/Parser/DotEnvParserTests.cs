using Sagi.Sdk.DotEnv.Parser;

namespace Sagi.Sdk.DotEnv.Tests.Parser;

public class DotEnvParserTests
{
    [Fact]
    public void Parse_ValidLine_ReturnsPair()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["KEY=VALUE"]);

        Assert.Equal("VALUE", result["KEY"]);
    }

    [Fact]
    public void Parse_EmptyLine_IsIgnored()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse([""]);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_WhitespaceLine_IsIgnored()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["   "]);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_CommentLine_IsIgnored()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["# this is a comment"]);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_LineWithoutEquals_IsIgnored()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["KEYONLY"]);

        Assert.Empty(result);
    }

    [Fact]
    public void Parse_ValueWithMultipleEquals_PreservesFullValue()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["KEY=a=b=c"]);

        Assert.Equal("a=b=c", result["KEY"]);
    }

    [Fact]
    public void Parse_KeyAndValueWithPadding_TrimsWhitespace()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["  KEY  =  VALUE  "]);

        Assert.True(result.ContainsKey("KEY"));
        Assert.Equal("VALUE", result["KEY"]);
    }

    [Fact]
    public void Parse_EmptyKeyAfterTrim_IsIgnored()
    {
        IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(["  =VALUE"]);

        Assert.Empty(result);
    }
}
