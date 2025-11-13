namespace ElectroShop.Application.Common.Results;

/// <summary>
/// Represents a result with validation errors
/// </summary>
public class ValidationResult : Result
{
    private ValidationResult(Error[] errors)
        : base(false, Error.Validation("Validation.Failed", "One or more validation errors occurred"))
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationResult WithErrors(params Error[] errors) => new(errors);

    public static ValidationResult WithErrors(IEnumerable<Error> errors) => new(errors.ToArray());
}

/// <summary>
/// Generic validation result with value
/// </summary>
public class ValidationResult<T> : Result<T>
{
    private ValidationResult(Error[] errors)
        : base(default, false, Error.Validation("Validation.Failed", "One or more validation errors occurred"))
    {
        Errors = errors;
    }

    public Error[] Errors { get; }

    public static ValidationResult<T> WithErrors(params Error[] errors) => new(errors);

    public static ValidationResult<T> WithErrors(IEnumerable<Error> errors) => new(errors.ToArray());
}

