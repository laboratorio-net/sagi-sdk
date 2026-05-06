using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Extensions;
using Sagi.Sdk.DotEnv.Provider;

namespace Sagi.Sdk.DotEnv.Tests.Extensions;

public class ConfigurationBuilderExtensionsTests : IDisposable
{
    private readonly string _tempDir;

    public ConfigurationBuilderExtensionsTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void AddDotEnv_NoArgs_AddsDotEnvSourceWithDefaults()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder();

        builder.AddDotEnv();

        Assert.Single(builder.Sources);
        Assert.IsType<DotEnvSource>(builder.Sources[0]);
    }

    [Fact]
    public void AddDotEnv_WithConfigure_AddsDotEnvSourceWithCustomOptions()
    {
        IConfigurationBuilder builder = new ConfigurationBuilder();

        builder.AddDotEnv(opt =>
        {
            opt.Directory = _tempDir;
            opt.FileName = ".env.custom";
        });

        Assert.Single(builder.Sources);
        Assert.IsType<DotEnvSource>(builder.Sources[0]);
    }

    [Fact]
    public void AddDotEnv_WithCustomDirectoryAndFileName_LoadsFromCombinedPath()
    {
        string envFile = Path.Combine(_tempDir, ".env.test");
        File.WriteAllLines(envFile, ["CUSTOM_KEY=CUSTOM_VALUE"]);

        IConfiguration config = new ConfigurationBuilder()
            .AddDotEnv(opt =>
            {
                opt.Directory = _tempDir;
                opt.FileName = ".env.test";
            })
            .Build();

        Assert.Equal("CUSTOM_VALUE", config["CUSTOM_KEY"]);
    }
}
