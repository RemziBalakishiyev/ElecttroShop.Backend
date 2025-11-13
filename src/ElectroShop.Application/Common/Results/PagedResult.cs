namespace ElectroShop.Application.Common.Results;

/// <summary>
/// Represents a paginated result with metadata
/// </summary>
/// <typeparam name="T">The type of items in the page</typeparam>
public class PagedResult<T> : Result<IReadOnlyList<T>>
{
    protected PagedResult(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount,
        bool isSuccess,
        Error error)
        : base(items, isSuccess, error)
    {
        Page = page;
        PageSize = pageSize;
        TotalCount = totalCount;
    }

    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public static PagedResult<T> Success(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalCount)
    {
        return new PagedResult<T>(items, page, pageSize, totalCount, true, Error.None);
    }

    public static new PagedResult<T> Failure(Error error)
    {
        return new PagedResult<T>(
            Array.Empty<T>(),
            0,
            0,
            0,
            false,
            error);
    }

    public static PagedResult<T> Empty(int page, int pageSize)
    {
        return Success(Array.Empty<T>(), page, pageSize, 0);
    }

    /// <summary>
    /// Maps the paged result items to another type while preserving pagination metadata
    /// </summary>
    public PagedResult<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        if (IsFailure)
            return PagedResult<TNew>.Failure(Error);

        var mappedItems = Value.Select(mapper).ToList();
        return PagedResult<TNew>.Success(mappedItems, Page, PageSize, TotalCount);
    }

    /// <summary>
    /// Gets pagination metadata
    /// </summary>
    public PagedMetadata GetMetadata() => new(
        Page,
        PageSize,
        TotalCount,
        TotalPages,
        HasPreviousPage,
        HasNextPage);
}

/// <summary>
/// Pagination metadata
/// </summary>
public record PagedMetadata(
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

/// <summary>
/// Pagination request parameters
/// </summary>
public record PagedRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;

    public static PagedRequest Default => new();
}

