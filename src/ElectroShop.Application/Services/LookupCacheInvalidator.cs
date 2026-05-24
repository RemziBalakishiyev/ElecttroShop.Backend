using ElectroShop.Application.Common;
using Microsoft.Extensions.Caching.Memory;

namespace ElectroShop.Application.Services;

public class LookupCacheInvalidator : ILookupCacheInvalidator
{
    private readonly IMemoryCache _memoryCache;

    public LookupCacheInvalidator(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public void InvalidateCategoriesLookup()
    {
        _memoryCache.Remove(LookupCacheKeys.Categories);
    }

    public void InvalidateBrandsLookup()
    {
        _memoryCache.Remove(LookupCacheKeys.Brands);
    }
}
