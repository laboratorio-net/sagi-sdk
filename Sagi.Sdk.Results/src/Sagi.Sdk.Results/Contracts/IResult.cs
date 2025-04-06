namespace Sagi.Sdk.Results.Contracts;

public interface IResult
{
    bool IsSuccess { get; }
    bool IsFailure { get; }
    IEnumerable<IError>? Errors { get; }
}

public interface IResult<T> : IResult
{
    T? Value { get; }
}
