using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Results;

public class Result<T> : IResult<T>
{
    public T? Value { get; }

    public bool IsSuccess => Errors == null || Errors.Any() == false;

    public bool IsFailure => !IsSuccess;

    public IEnumerable<IError>? Errors { get; }

    public Result(T value)
        => Value = value ?? throw new ArgumentNullException(nameof(value));

    public Result(IEnumerable<IError> errors)
        => Errors = errors ?? throw new ArgumentNullException(nameof(errors));
}
