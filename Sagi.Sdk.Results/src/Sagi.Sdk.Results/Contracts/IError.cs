namespace Sagi.Sdk.Results.Contracts;

public interface IError
{
    public string Code { get; }
    public string Message { get; }
}
