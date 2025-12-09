using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetPromotionalBrands;

public class GetPromotionalBrandsQueryHandler : IRequestHandler<GetPromotionalBrandsQuery, Result<List<PromotionalBrandDto>>>
{
    private readonly IBrandQueryRepository _brandRepository;
    private readonly IProductQueryRepository _productRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetPromotionalBrandsQueryHandler(
        IBrandQueryRepository brandRepository,
        IProductQueryRepository productRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _brandRepository = brandRepository;
        _productRepository = productRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<List<PromotionalBrandDto>>> Handle(
        GetPromotionalBrandsQuery request,
        CancellationToken cancellationToken)
    {
        // Promotional brendləri əldə et (maksimum 4)
        var promotionalBrands = await _brandRepository.GetPromotionalBrandsAsync(cancellationToken);

        if (promotionalBrands.Count == 0)
        {
            return Result.Success(new List<PromotionalBrandDto>());
        }

        var result = new List<PromotionalBrandDto>();

        foreach (var brand in promotionalBrands)
        {
            // Hər brend üçün featured məhsul tap
            var featuredProduct = await _productRepository.GetFeaturedProductByBrandIdAsync(
                brand.Id,
                cancellationToken);

            // Əgər featured product yoxdursa, bu brendi skip et
            if (featuredProduct == null)
                continue;

            // Brend üçün endirim faizini hesabla
            var brandDiscountPercent = await _discountCalculationService.GetBrandDiscountPercentAsync(
                brand.Id,
                cancellationToken);

            // Məhsul üçün final endirim faizini hesabla
            var productDiscountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
                featuredProduct.Id,
                featuredProduct.CategoryId,
                featuredProduct.BrandId,
                cancellationToken);

            // Endirimli qiyməti hesabla
            var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
                featuredProduct.Price.Amount,
                productDiscountPercent);

            // ProductDto yarat
            var productDto = featuredProduct.Adapt<ProductDto>();
            productDto = productDto with
            {
                FinalDiscountPercent = productDiscountPercent,
                FinalPrice = finalPrice,
                CategoryName = featuredProduct.Category?.Name ?? string.Empty,
                BrandName = featuredProduct.Brand?.Name ?? string.Empty,
                Sku = featuredProduct.Sku.Value,
                Price = featuredProduct.Price.Amount,
                Currency = featuredProduct.Price.Currency,
                CreatedAt = featuredProduct.CreatedAtUtc,
                UpdatedAt = featuredProduct.UpdatedAtUtc
            };

            // BrandInfoDto yarat
            var brandInfoDto = new BrandInfoDto
            {
                Id = brand.Id,
                Name = brand.Name,
                DiscountPercent = brandDiscountPercent,
                CreatedAt = brand.CreatedAtUtc
            };

            // PromotionalBrandDto yarat
            result.Add(new PromotionalBrandDto
            {
                Brand = brandInfoDto,
                FeaturedProduct = productDto
            });
        }

        return Result.Success(result);
    }
}


