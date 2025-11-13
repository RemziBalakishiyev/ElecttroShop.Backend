namespace ElectroShop.Application.Common.Results;

/// <summary>
/// Non-generic Result class for operations that don't return a value
/// </summary>
public class Result
{
    protected Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None)
            throw new InvalidOperationException("Success result cannot have an error");

        if (!isSuccess && error == Error.None)
            throw new InvalidOperationException("Failure result must have an error");

        IsSuccess = isSuccess;
        Error = error;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }

    public static Result Success() => new(true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<T> Success<T>(T value) => new(value, true, Error.None);

    public static Result<T> Failure<T>(Error error) => new(default, false, error);

    public static Result<T> Create<T>(T? value) =>
        value is not null ? Success(value) : Failure<T>(Error.NullValue);

    /// <summary>
    /// Combines multiple results into one. If any result fails, returns the first failure.
    /// </summary>
    public static Result Combine(params Result[] results)
    {
        foreach (var result in results)
        {
            if (result.IsFailure)
                return result;
        }

        return Success();
    }

    /// <summary>
    /// Combines multiple results. Returns all errors if any failures exist.
    /// </summary>
    public static Result CombineAll(params Result[] results)
    {
        var failures = results.Where(r => r.IsFailure).ToList();

        if (!failures.Any())
            return Success();

        var errors = failures.Select(f => f.Error).ToArray();
        return Failure(Error.Failure(
            "Multiple.Errors",
            $"Multiple errors occurred: {string.Join(", ", errors.Select(e => e.Code))}"));
    }
}

/// <summary>
/// Generic Result class for operations that return a value
/// </summary>
/// <typeparam name="T">The type of the value</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result");

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure<T>(error);

    /// <summary>
    /// Maps the result value to another type
    /// </summary>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess
            ? Success(mapper(Value))
            : Failure<TNew>(Error);
    }

    /// <summary>
    /// Binds the result to another result-returning operation
    /// </summary>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> func)
    {
        return IsSuccess
            ? func(Value)
            : Failure<TNew>(Error);
    }

    /// <summary>
    /// Matches the result to one of two functions based on success/failure
    /// </summary>
    public TResult Match<TResult>(
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error);
    }

    /// <summary>
    /// Executes an action if the result is successful
    /// </summary>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
            action(Value);

        return this;
    }

    /// <summary>
    /// Executes an action if the result is a failure
    /// </summary>
    public Result<T> OnFailure(Action<Error> action)
    {
        if (IsFailure)
            action(Error);

        return this;
    }

    /// <summary>
    /// Returns the value if successful, otherwise returns the default value
    /// </summary>
    public T? ValueOrDefault(T? defaultValue = default) =>
        IsSuccess ? Value : defaultValue;
}

