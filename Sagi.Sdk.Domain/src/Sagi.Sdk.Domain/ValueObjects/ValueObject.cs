using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.ValueObjects;

public abstract class ValueObject
{
    private readonly List<IError> _errors = [];

    public IReadOnlyList<IError> Errors => [.. _errors.AsReadOnly()];
    public bool IsInvalid => !IsValid;
    public bool IsValid => _errors.Count == 0;

    public abstract void Validate();

    protected void AddError(IError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));
        _errors.Add(error);
    }

    protected void ClearErrors() => _errors.Clear();
}