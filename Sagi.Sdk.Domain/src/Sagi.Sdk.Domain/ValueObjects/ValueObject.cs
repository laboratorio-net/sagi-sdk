using Sagi.Sdk.Domain.Contracts;
using Sagi.Sdk.Results;

namespace Sagi.Sdk.Domain.ValueObjects;

public abstract class ValueObject<TChild> : Validateble, IEquatable<TChild>
{
    public abstract bool Equals(TChild? other);
    public abstract override bool Equals(object? obj);
    public abstract override int GetHashCode();
    public abstract override string ToString();

    protected void Validate(
        Validateble? valueObject,
        string errorCode,
        string nullErrorMessage)
    {
        if (valueObject is null)
        {
            AddError(new Error(errorCode, nullErrorMessage));
        }
        else
        {
            valueObject.Validate();
            if (valueObject.IsInvalid)
            {
                var voErrorCode = $"{errorCode}_{valueObject.GetType().Name.ToUpper()}";
                AddErrors(valueObject.Errors.Select(e =>
                    new Error(voErrorCode, e.Message)));
            }
        }
    }
}