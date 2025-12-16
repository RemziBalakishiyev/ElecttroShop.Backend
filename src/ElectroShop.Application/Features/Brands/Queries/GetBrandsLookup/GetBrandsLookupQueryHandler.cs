using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrandsLookup;

public class GetBrandsLookupQueryHandler : IRequestHandler<GetBrandsLookupQuery, Result<LookupResponse>>
{
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IMemoryCache _memoryCache;
    private const string CacheKey = "BrandsLookup";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromHours(1);

    public GetBrandsLookupQueryHandler(
        IQueryRepository<Brand> brandRepository,
        IMemoryCache memoryCache)
    {
        _brandRepository = brandRepository;
        _memoryCache = memoryCache;
    }

    public async Task<Result<LookupResponse>> Handle(
        GetBrandsLookupQuery request,
        CancellationToken cancellationToken)
    {
        // Cache-dən yoxla
        if (_memoryCache.TryGetValue(CacheKey, out LookupResponse? cachedResponse) && cachedResponse != null)
        {
            return Result.Success(cachedResponse);
        }

        // Database-dən yüklə
        var brands = await _brandRepository.FindAsync(
            b => !b.IsDeleted,
            cancellationToken);

        var items = brands
            .OrderBy(b => b.Name)
            .Select(b => new LookupDto
            {
                Key = b.Id.ToString(),
                Value = b.Name
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

