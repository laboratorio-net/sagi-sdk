using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Options;
using Sagi.Sdk.DotEnv.Provider;

namespace Sagi.Sdk.DotEnv.Tests.Provider;

public class DotEnvSourceTests
{
    [Fact]
    public void Constructor_NullDirectory_ThrowsArgumentException()
    {
        DotEnvOptions options = new() { Directory = null! };

        Assert.ThrowsAny<ArgumentException>(() => new DotEnvSource(options));
    }

    [Fact]
    public void Constructor_EmptyDirectory_ThrowsArgumentException()
    {
        DotEnvOptions options = new() { Directory = string.Empty };

        Assert.ThrowsAny<ArgumentException>(() => new DotEnvSource(options));
    }

    [Fact]
    public void Constructor_NullFileName_ThrowsArgumentException()
    {
        DotEnvOptions options = new() { FileName = null! };

        Assert.ThrowsAny<ArgumentException>(() => new DotEnvSource(options));
    }

    [Fact]
    public void Constructor_EmptyFileName_ThrowsArgumentException()
    {
        DotEnvOptions options = new() { FileName = string.Empty };

        Assert.ThrowsAny<ArgumentException>(() => new DotEnvSource(options));
    }

    [Fact]
    public void Build_ValidOptions_ReturnsDotEnvProvider()
    {
        DotEnvOptions options = new();
        DotEnvSource source = new(options);
        IConfigurationBuilder builder = new ConfigurationBuilder();

        IConfigurationProvider provider = source.Build(builder);

        Assert.IsType<DotEnvProvider>(provider);
    }
}
