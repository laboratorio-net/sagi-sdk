namespace Sagi.Sdk.DotEnv.Parser;

public static class DotEnvParser
{
    public static IReadOnlyDictionary<string, string> Parse(IEnumerable<string> lines)
    {
        Dictionary<string, string> result = new(StringComparer.OrdinalIgnoreCase);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string trimmed = line.Trim();

            if (trimmed.StartsWith('#'))
                continue;

            if (!trimmed.Contains('='))
                continue;

            string[] parts = trimmed.Split('=', 2);
            string key = parts[0].Trim();
            string value = parts[1].Trim();

            if (string.IsNullOrEmpty(key))
                continue;

            result[key] = value;
        }

        return result;
    }
}
