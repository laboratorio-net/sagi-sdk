using System.Text.Json.Serialization;

using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Results;

public class Error : IError
{
    public string Code { get; }
    public string Message { get; }

    [JsonConstructor]
    public Error(string code, string message)
    {
        ArgumentException.ThrowIfNullOrEmpty(code, nameof(code));
        Code = code;

        ArgumentException.ThrowIfNullOrEmpty(message, nameof(message));
        Message = message;
    }
}
