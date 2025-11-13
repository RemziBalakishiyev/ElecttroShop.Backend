namespace ElectroShop.Application.Common.Results;

/// <summary>
/// Extension methods for Result pattern
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Converts a list of results into a single result containing a list
    /// </summary>
    public static Result<IReadOnlyList<T>> Combine<T>(this IEnumerable<Result<T>> results)
    {
        var resultsList = results.ToList();
        
        var failures = resultsList.Where(r => r.IsFailure).ToList();
        if (failures.Any())
        {
            var firstError = failures.First().Error;
            return Result.Failure<IReadOnlyList<T>>(firstError);
        }

        var values = resultsList.Select(r => r.Value).ToList();
        return Result.Success<IReadOnlyList<T>>(values);
    }

    /// <summary>
    /// Executes an async action if the result is successful
    /// </summary>
    public static async Task<Result<T>> OnSuccessAsync<T>(
        this Result<T> result,
        Func<T, Task> action)
    {
        if (result.IsSuccess)
            await action(result.Value);

        return result;
    }

    /// <summary>
    /// Executes an async action if the result is a failure
    /// </summary>
    public static async Task<Result<T>> OnFailureAsync<T>(
        this Result<T> result,
        Func<Error, Task> action)
    {
        if (result.IsFailure)
            await action(result.Error);

        return result;
    }

    /// <summary>
    /// Maps the result value to another type asynchronously
    /// </summary>
    public static async Task<Result<TNew>> MapAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<TNew>> mapper)
    {
        return result.IsSuccess
            ? Result.Success(await mapper(result.Value))
            : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Binds the result to another async result-returning operation
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Result<T> result,
        Func<T, Task<Result<TNew>>> func)
    {
        return result.IsSuccess
            ? await func(result.Value)
            : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Binds the async result to another async result-returning operation
    /// </summary>
    public static async Task<Result<TNew>> BindAsync<T, TNew>(
        this Task<Result<T>> resultTask,
        Func<T, Task<Result<TNew>>> func)
    {
        var result = await resultTask;
        return result.IsSuccess
            ? await func(result.Value)
            : Result.Failure<TNew>(result.Error);
    }

    /// <summary>
    /// Matches the async result to one of two functions
    /// </summary>
    public static async Task<TResult> MatchAsync<T, TResult>(
        this Task<Result<T>> resultTask,
        Func<T, TResult> onSuccess,
        Func<Error, TResult> onFailure)
    {
        var result = await resultTask;
        return result.Match(onSuccess, onFailure);
    }

    /// <summary>
    /// Ensures a condition is met, otherwise returns a failure
    /// </summary>
    public static Result<T> Ensure<T>(
        this Result<T> result,
        Func<T, bool> predicate,
        Error error)
    {
        if (result.IsFailure)
            return result;

        return predicate(result.Value)
            ? result
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Ensures an async condition is met, otherwise returns a failure
    /// </summary>
    public static async Task<Result<T>> EnsureAsync<T>(
        this Result<T> result,
        Func<T, Task<bool>> predicate,
        Error error)
    {
        if (result.IsFailure)
            return result;

        return await predicate(result.Value)
            ? result
            : Result.Failure<T>(error);
    }

    /// <summary>
    /// Taps into the result for side effects without modifying it
    /// </summary>
    public static Result<T> Tap<T>(this Result<T> result, Action<T> action)
    {
        if (result.IsSuccess)
            action(result.Value);

        return result;
    }

    /// <summary>
    /// Taps into the async result for side effects without modifying it
    /// </summary>
    public static async Task<Result<T>> TapAsync<T>(
        this Result<T> result,
        Func<T, Task> action)
    {
        if (result.IsSuccess)
            await action(result.Value);

        return result;
    }

    /// <summary>
    /// Converts nullable value to Result
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : class
    {
        return value is not null ? Result.Success(value) : Result.Failure<T>(error);
    }

    /// <summary>
    /// Converts nullable struct to Result
    /// </summary>
    public static Result<T> ToResult<T>(this T? value, Error error) where T : struct
    {
        return value.HasValue ? Result.Success(value.Value) : Result.Failure<T>(error);
    }

    /// <summary>
    /// Converts Task Result to ValueTask Result
    /// </summary>
    public static async ValueTask<Result<T>> ToValueTask<T>(this Task<Result<T>> task)
    {
        return await task;
    }
}

