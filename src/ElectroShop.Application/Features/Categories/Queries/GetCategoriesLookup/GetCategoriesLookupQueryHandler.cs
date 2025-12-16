using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoriesLookup;

public class GetCategoriesLookupQueryHandler : IRequestHandler<GetCategoriesLookupQuery, Result<LookupResponse>>
{
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "CategoriesLookup";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public GetCategoriesLookupQueryHandler(
        IQueryRepository<Category> categoryRepository,
        IMemoryCache memoryCache)
    {
        _categoryRepository = categoryRepository;
        _memoryCache = memoryCache;
    }

    public async Task<Result<LookupResponse>> Handle(
        GetCategoriesLookupQuery request,
        CancellationToken cancellationToken)
    {
        // Cache-dən yoxla
        if (_memoryCache.TryGetValue(CacheKey, out LookupResponse? cachedResponse) && cachedResponse != null)
        {
            return Result.Success(cachedResponse);
        }

        // Database-dən yüklə
        var categories = await _categoryRepository.FindAsync(
            c => !c.IsDeleted,
            cancellationToken);

        var items = categories
            .OrderBy(c => c.Name)
            .Select(c => new LookupDto
            {
                Key = c.Id.ToString(),
                Value = c.Name
            })
            .ToList();

        var response = new LookupResponse
        {
            Items = items,
            CachedAt = DateTime.UtcNow,
            CacheKey = CacheKey
        };

        // Cache-ə yaz
        _memoryCache.Set(CacheKey, response, CacheExpiration);

        return Result.Success(response);
    }
}

