using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Options;
using Sagi.Sdk.DotEnv.Provider;

namespace Sagi.Sdk.DotEnv.Extensions;

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddDotEnv(this IConfigurationBuilder builder)
        => builder.AddDotEnv(_ => { });

    public static IConfigurationBuilder AddDotEnv(
        this IConfigurationBuilder builder,
        Action<DotEnvOptions> configure)
    {
        DotEnvOptions options = new();
        configure(options);
        return builder.Add(new DotEnvSource(options));
    }
}
