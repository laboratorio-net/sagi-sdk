using Sagi.Sdk.DotEnv.Options;
using Sagi.Sdk.DotEnv.Provider;

namespace Sagi.Sdk.DotEnv.Tests.Provider;

public class DotEnvProviderTests : IDisposable
{
    private readonly string _tempDir;

    public DotEnvProviderTests()
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
    public void Load_ExistingFileWithValidEntries_PopulatesData()
    {
        string envFile = Path.Combine(_tempDir, ".env");
        File.WriteAllLines(envFile, ["KEY1=VALUE1", "KEY2=VALUE2"]);

        DotEnvOptions options = new() { Directory = _tempDir, FileName = ".env" };
        DotEnvProvider provider = new(options);

        provider.Load();

        Assert.True(provider.TryGet("KEY1", out string? val1));
        Assert.Equal("VALUE1", val1);
        Assert.True(provider.TryGet("KEY2", out string? val2));
        Assert.Equal("VALUE2", val2);
    }

    [Fact]
    public void Load_NonExistentFile_DoesNotThrow_AndDataIsEmpty()
    {
        DotEnvOptions options = new() { Directory = _tempDir, FileName = ".env.missing" };
        DotEnvProvider provider = new(options);

        Exception? ex = Record.Exception(() => provider.Load());

        Assert.Null(ex);
        Assert.False(provider.TryGet("ANY", out _));
    }

    [Fact]
    public void Load_EmptyFile_DoesNotThrow_AndDataIsEmpty()
    {
        string envFile = Path.Combine(_tempDir, ".env");
        File.WriteAllText(envFile, string.Empty);

        DotEnvOptions options = new() { Directory = _tempDir, FileName = ".env" };
        DotEnvProvider provider = new(options);

        Exception? ex = Record.Exception(() => provider.Load());

        Assert.Null(ex);
        Assert.False(provider.TryGet("ANY", out _));
    }

    [Fact]
    public void Load_HierarchicalKeys_ConvertsDoubleUnderscoreToColon()
    {
        string envFile = Path.Combine(_tempDir, ".env");
        File.WriteAllLines(envFile, [
            "Database__Host=localhost",
            "Database__Port=5432",
            "Database__Credentials__Username=admin",
            "Database__Credentials__Password=secret"
        ]);

        DotEnvOptions options = new() { Directory = _tempDir, FileName = ".env" };
        DotEnvProvider provider = new(options);

        provider.Load();

        Assert.True(provider.TryGet("Database:Host", out string? host));
        Assert.Equal("localhost", host);
        Assert.True(provider.TryGet("Database:Port", out string? port));
        Assert.Equal("5432", port);
        Assert.True(provider.TryGet("Database:Credentials:Username", out string? username));
        Assert.Equal("admin", username);
        Assert.True(provider.TryGet("Database:Credentials:Password", out string? password));
        Assert.Equal("secret", password);
    }
}
