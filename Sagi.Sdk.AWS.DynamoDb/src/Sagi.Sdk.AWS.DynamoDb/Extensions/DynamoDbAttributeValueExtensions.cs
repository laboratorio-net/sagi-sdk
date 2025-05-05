using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.DynamoDBv2.Model;

namespace Sagi.Sdk.AWS.DynamoDb.Extensions;

public static class DynamoDbAttributeValueExtensions
{
    public static JsonSerializerOptions JsonOptions
        => new() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault };

    public static Dictionary<string, AttributeValue>? Deserialize(this string? token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return default;
        }

        var bytes = Convert.FromBase64String(token);
        return JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(bytes)!;
    }

    public static string? Serialize(this Dictionary<string, AttributeValue>? token)
    {
        if (token is { Count: > 0 })
        {
            var bytes = JsonSerializer.SerializeToUtf8Bytes(token, JsonOptions);
            return Convert.ToBase64String(bytes);
        }

        return null;
    }
}


