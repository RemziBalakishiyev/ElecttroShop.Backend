using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly ICategoryQueryRepository _categoryRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetProductByIdQueryHandler(
        IProductQueryRepository productRepository,
        ICategoryQueryRepository categoryRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        var productDto = product.Adapt<ProductDto>();

        // Variants-ı manual set et (çünki mapping-də ignore etdik)
        productDto = productDto with
        {
            Variants = product.ProductVariants.Select(pv =>
            {
                var attributes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(pv.AttributesJson);
                return new ProductVariantDto
                {
                    Id = pv.Id,
                    Sku = pv.Sku.Value,
                    Price = pv.Price.Amount,
                    Currency = pv.Price.Currency,
                    Stock = pv.Stock,
                    IsActive = pv.IsActive,
                    ImageId = pv.ImageId,
                    ImageUrl = pv.ImageId.HasValue ? $"/api/images/{pv.ImageId.Value}" : null,
                    Attributes = attributes ?? new Dictionary<string, string>()
                };
            }).ToList()
        };

        // Endirim hesablaması
        var discountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
            product.Id,
            product.CategoryId,
            product.BrandId,
            cancellationToken);

        var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
            product.Price.Amount,
            discountPercent);

        // Kateqoriya atributlarını yüklə
        var categoryAttributes = await _categoryRepository.GetCategoryAttributesAsync(
            product.CategoryId, 
            cancellationToken);

        // Variantlar üçün endirim hesabla
        var variantsWithDiscounts = new List<ProductVariantDto>();
        foreach (var variant in productDto.Variants)
        {
            var variantDiscountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
                product.Id,
                product.CategoryId,
                product.BrandId,
                cancellationToken);

            var variantFinalPrice = _discountCalculationService.CalculateDiscountedPrice(
                variant.Price,
                variantDiscountPercent);

            variantsWithDiscounts.Add(variant with
            {
                FinalDiscountPercent = variantDiscountPercent,
                FinalPrice = variantFinalPrice
            });
        }

        // Endirim məlumatlarını və kateqoriya atributlarını DTO-ya əlavə et
        productDto = productDto with
        {
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice,
            CategoryAttributes = categoryAttributes.Select(ca => new CategoryAttributeDto
            {
                Id = ca.Id,
                Name = ca.Name,
                DisplayName = ca.DisplayName,
                AttributeType = ca.AttributeType,
                IsRequired = ca.IsRequired,
                DisplayOrder = ca.DisplayOrder,
                Values = ca.Values.Select(cav => new CategoryAttributeValueDto
                {
                    Id = cav.Id,
                    Value = cav.Value,
                    DisplayValue = cav.DisplayValue,
                    DisplayOrder = cav.DisplayOrder,
                    ColorCode = cav.ColorCode
                }).OrderBy(v => v.DisplayOrder).ToList()
            }).OrderBy(ca => ca.DisplayOrder).ToList(),
            Variants = variantsWithDiscounts
        };

        return Result.Success(productDto);
    }
}





