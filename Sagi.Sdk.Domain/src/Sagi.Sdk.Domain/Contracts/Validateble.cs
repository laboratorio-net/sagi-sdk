using Sagi.Sdk.Results.Contracts;

namespace Sagi.Sdk.Domain.Contracts;

public abstract class Validateble
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

    protected void AddErrors(IEnumerable<IError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));
        _errors.AddRange(errors);
    }

    protected void ClearErrors() => _errors.Clear();
}