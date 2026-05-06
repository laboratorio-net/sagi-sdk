using Sagi.Sdk.DotEnv.Options;

namespace Sagi.Sdk.DotEnv.Tests.Options;

public class DotEnvOptionsTests
{
    [Fact]
    public void Directory_Default_IsCurrentDirectory()
    {
        DotEnvOptions options = new();

        Assert.Equal(System.IO.Directory.GetCurrentDirectory(), options.Directory);
    }

    [Fact]
    public void FileName_Default_IsDotEnv()
    {
        DotEnvOptions options = new();

        Assert.Equal(".env", options.FileName);
    }
}
