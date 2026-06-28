using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoriesLookup;

public class GetCategoriesLookupQueryHandler : IRequestHandler<GetCategoriesLookupQuery, Result<LookupResponse>>
{
    private readonly ICategoryQueryRepository _categoryRepository;
    private readonly IMemoryCache _memoryCache;
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public GetCategoriesLookupQueryHandler(
        ICategoryQueryRepository categoryRepository,
        IMemoryCache memoryCache)
    {
        _categoryRepository = categoryRepository;
        _memoryCache = memoryCache;
    }

    public async Task<Result<LookupResponse>> Handle(
        GetCategoriesLookupQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = LookupCacheKeys.GetCategoriesLookupKey(request.IncludeAll, request.ParentId);
        var useCache = !request.ParentId.HasValue;

        if (useCache && _memoryCache.TryGetValue(cacheKey, out LookupResponse? cachedResponse) && cachedResponse != null)
        {
            return Result.Success(cachedResponse);
        }

        var categories = await _categoryRepository.GetCategoriesForLookupAsync(
            request.IncludeAll,
            request.ParentId,
            cancellationToken);

        var items = categories
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
            CacheKey = cacheKey
        };

        if (useCache)
        {
            _memoryCache.Set(cacheKey, response, CacheExpiration);
        }

        return Result.Success(response);
    }
}
