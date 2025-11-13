namespace ElectroShop.Application.Common.Pagination;

/// <summary>
/// Pagination extension methods
/// IQueryable üçün dinamik pagination
/// </summary>
public static class PaginationExtensions
{
    /// <summary>
    /// Səhifələmə tətbiq et
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        int page,
        int pageSize)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        return query
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
    }

    /// <summary>
    /// Səhifələmə tətbiq et (PagedRequest istifadə edərək)
    /// </summary>
    public static IQueryable<T> ApplyPagination<T>(
        this IQueryable<T> query,
        PagedRequest pagedRequest)
    {
        return query.ApplyPagination(pagedRequest.Page, pagedRequest.PageSize);
    }
}

/// <summary>
/// Pagination request parametrləri
/// </summary>
public record PagedRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 10;

    public int Page { get; init; } = 1;

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 10 : value;
    }

    public int Skip => (Page - 1) * PageSize;
    public int Take => PageSize;

    public static PagedRequest Default => new();
    
    public static PagedRequest Create(int page, int pageSize) => new() { Page = page, PageSize = pageSize };
}

