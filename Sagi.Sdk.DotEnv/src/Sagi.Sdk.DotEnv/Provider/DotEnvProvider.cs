using Microsoft.Extensions.Configuration;
using Sagi.Sdk.DotEnv.Options;
using Sagi.Sdk.DotEnv.Parser;

namespace Sagi.Sdk.DotEnv.Provider;

public class DotEnvProvider : ConfigurationProvider
{
    private readonly DotEnvOptions _options;

    public DotEnvProvider(DotEnvOptions options)
        => _options = options;

    public override void Load()
    {
        string filePath = Path.Combine(_options.Directory, _options.FileName);

        if (!File.Exists(filePath))
            return;

        IEnumerable<string> lines = File.ReadAllLines(filePath);
        IReadOnlyDictionary<string, string> entries = DotEnvParser.Parse(lines);

        Data = new Dictionary<string, string?>(
            entries.ToDictionary(
                kvp => kvp.Key.Replace("__", ConfigurationPath.KeyDelimiter),
                kvp => (string?)kvp.Value),
            StringComparer.OrdinalIgnoreCase);
    }
}
