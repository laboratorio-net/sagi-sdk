using FsCheck;
using FsCheck.Xunit;
using Sagi.Sdk.DotEnv.Parser;

namespace Sagi.Sdk.DotEnv.Tests.Parser;

public class DotEnvParserPropertyTests
{
    // Valid key: non-empty, no '=', not whitespace-only, no '#', no control chars
    private static Gen<string> ValidKeyGen =>
        Arb.Generate<NonEmptyString>()
            .Where(s =>
                !s.Get.Contains('=') &&
                !string.IsNullOrWhiteSpace(s.Get) &&
                !s.Get.TrimStart().StartsWith('#') &&
                !s.Get.Any(char.IsControl))
            .Select(s => s.Get.Trim());

    private static Gen<string> ValidValueGen =>
        Arb.Generate<string>()
            .Select(s => (s ?? string.Empty).Trim())
            .Where(s => !s.Any(char.IsControl));

    private static Gen<(string Key, string Value)> ValidPairGen =>
        from key in ValidKeyGen
        from value in ValidValueGen
        select (key, value);

    // Feature: dotenv-sdk, Property 1: Round-trip de parsing
    // Validates: Requirements 1.3, 3.1, 3.7, 4.3, 5.6
    [Property(MaxTest = 100)]
    public Property RoundTrip_ParseAfterSerialize_ReturnsSamePairs()
    {
        Arbitrary<List<(string Key, string Value)>> arb =
            Arb.From(Gen.ListOf(ValidPairGen).Select(p => p.ToList()));

        return Prop.ForAll(arb, pairs =>
        {
            // Last write wins for duplicate keys
            Dictionary<string, string> expected = new(StringComparer.OrdinalIgnoreCase);
            foreach ((string key, string value) in pairs)
                expected[key] = value;

            IEnumerable<string> lines = pairs.Select(p => $"{p.Key}={p.Value}");
            IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(lines);

            return expected.All(kvp =>
                result.TryGetValue(kvp.Key, out string? actual) && actual == kvp.Value);
        });
    }

    // Feature: dotenv-sdk, Property 2: Valores com = são preservados integralmente
    // Validates: Requirements 3.5
    [Property(MaxTest = 100)]
    public Property ValuesWithEquals_ArePreservedIntegrally()
    {
        Arbitrary<(string Key, string Value)> arb = Arb.From(
            from key in ValidKeyGen
            from value in Arb.Generate<NonEmptyString>().Select(s => s.Get.Trim() + "=extra")
            select (key, value)
        );

        return Prop.ForAll(arb, pair =>
        {
            (string key, string value) = pair;
            IReadOnlyDictionary<string, string> result = DotEnvParser.Parse([$"{key}={value}"]);

            return result.TryGetValue(key, out string? actual) && actual == value;
        });
    }

    // Feature: dotenv-sdk, Property 3: Linhas inválidas não afetam o resultado
    // Validates: Requirements 3.2, 3.3, 3.4
    [Property(MaxTest = 100)]
    public Property InvalidLines_DoNotAffectValidEntries()
    {
        Gen<string> invalidLineGen = Gen.OneOf(
            Gen.Constant(string.Empty),
            Gen.Constant("   "),
            Arb.Generate<NonEmptyString>().Select(s => "# " + s.Get),
            Arb.Generate<NonEmptyString>()
                .Where(s => !s.Get.Contains('='))
                .Select(s => s.Get)
        );

        Arbitrary<(List<(string Key, string Value)> Valid, List<string> Invalid)> arb = Arb.From(
            from valid in Gen.ListOf(ValidPairGen).Select(p => p.ToList())
            from invalid in Gen.ListOf(invalidLineGen).Select(p => p.ToList())
            select (valid, invalid)
        );

        return Prop.ForAll(arb, data =>
        {
            (List<(string Key, string Value)> validPairs, List<string> invalidLines) = data;

            Dictionary<string, string> expected = new(StringComparer.OrdinalIgnoreCase);
            foreach ((string key, string value) in validPairs)
                expected[key] = value;

            List<string> allLines = validPairs
                .Select(p => $"{p.Key}={p.Value}")
                .Concat(invalidLines)
                .ToList();

            IReadOnlyDictionary<string, string> result = DotEnvParser.Parse(allLines);

            return expected.All(kvp =>
                result.TryGetValue(kvp.Key, out string? actual) && actual == kvp.Value);
        });
    }

    // Feature: dotenv-sdk, Property 4: Idempotência do parsing
    // Validates: Requirements 3.1, 3.7
    [Property(MaxTest = 100)]
    public Property Parse_CalledTwice_ReturnsSamePairs()
    {
        Gen<string> anyLineGen = Gen.OneOf(
            ValidPairGen.Select(p => $"{p.Key}={p.Value}"),
            Gen.Constant(string.Empty),
            Arb.Generate<NonEmptyString>().Select(s => "# " + s.Get)
        );

        Arbitrary<List<string>> arb =
            Arb.From(Gen.ListOf(anyLineGen).Select(l => l.ToList()));

        return Prop.ForAll(arb, lines =>
        {
            IReadOnlyDictionary<string, string> first = DotEnvParser.Parse(lines);
            IReadOnlyDictionary<string, string> second = DotEnvParser.Parse(lines);

            return first.Count == second.Count &&
                   first.All(kvp => second.TryGetValue(kvp.Key, out string? v) && v == kvp.Value);
        });
    }
}
