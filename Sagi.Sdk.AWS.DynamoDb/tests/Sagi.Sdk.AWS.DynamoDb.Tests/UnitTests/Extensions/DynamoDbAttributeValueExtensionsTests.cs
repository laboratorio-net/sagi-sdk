using System.Text.Json;

using Amazon.DynamoDBv2.Model;
using Sagi.Sdk.AWS.DynamoDb.Extensions;

namespace Sagi.Sdk.AWS.DynamoDb.Tests.UnitTests.Extensions;

public class DynamoDbAttributeValueExtensionsTests
{
    [Fact]
    public void Deserialize_ValidBase64String_ReturnsDictionary()
    {
        var attributeValue = new AttributeValue { S = "TestValue" };
        var dictionary = new Dictionary<string, AttributeValue> { { "Key", attributeValue } };
        var json = JsonSerializer.Serialize(dictionary);
        var base64String = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(json));

        var result = base64String.Deserialize();

        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("TestValue", result["Key"].S);
    }

    [Fact]
    public void Deserialize_NullOrEmptyString_ReturnsNull()
    {
        string? nullString = null;
        string emptyString = string.Empty;

        var resultFromNull = nullString.Deserialize();
        var resultFromEmpty = emptyString.Deserialize();

        Assert.Null(resultFromNull);
        Assert.Null(resultFromEmpty);
    }

    [Fact]
    public void Serialize_ValidDictionary_ReturnsBase64String()
    {
        var attributeValue = new AttributeValue { S = "TestValue" };
        var dictionary = new Dictionary<string, AttributeValue> { { "Key", attributeValue } };

        var result = dictionary.Serialize();

        Assert.NotNull(result);
        var decodedJson = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(result!));
        var deserializedDictionary = JsonSerializer.Deserialize<Dictionary<string, AttributeValue>>(decodedJson);
        Assert.NotNull(deserializedDictionary);
        Assert.Single(deserializedDictionary);
        Assert.Equal("TestValue", deserializedDictionary["Key"].S);
    }

    [Fact]
    public void Serialize_NullOrEmptyDictionary_ReturnsNull()
    {
        Dictionary<string, AttributeValue>? nullDictionary = null;
        var emptyDictionary = new Dictionary<string, AttributeValue>();

        var resultFromNull = nullDictionary.Serialize();
        var resultFromEmpty = emptyDictionary.Serialize();

        Assert.Null(resultFromNull);
        Assert.Null(resultFromEmpty);
    }
}