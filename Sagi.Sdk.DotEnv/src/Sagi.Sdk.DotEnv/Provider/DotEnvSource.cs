using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Options;

namespace Sagi.Sdk.DotEnv.Provider;

public class DotEnvSource : IConfigurationSource
{
    private readonly DotEnvOptions _options;

    public DotEnvSource(DotEnvOptions options)
    {
        ArgumentException.ThrowIfNullOrEmpty(options.Directory);
        ArgumentException.ThrowIfNullOrEmpty(options.FileName);
        _options = options;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
        => new DotEnvProvider(_options);
}
