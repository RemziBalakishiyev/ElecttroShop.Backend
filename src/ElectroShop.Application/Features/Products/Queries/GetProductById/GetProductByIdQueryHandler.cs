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
    private readonly IProductRatingQueryRepository _ratingRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IImageStorage _imageStorage;
    private readonly ICurrentUserService _currentUserService;

    public GetProductByIdQueryHandler(
        IProductQueryRepository productRepository,
        ICategoryQueryRepository categoryRepository,
        IProductRatingQueryRepository ratingRepository,
        IDiscountCalculationService discountCalculationService,
        IImageStorage imageStorage,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _ratingRepository = ratingRepository;
        _discountCalculationService = discountCalculationService;
        _imageStorage = imageStorage;
        _currentUserService = currentUserService;
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

        // PrimaryImageUrl-i set et - ilk şəkil və ya primary şəkil
        var primaryImage = product.ProductImages
            .OrderBy(pi => pi.IsPrimary ? 0 : 1)
            .ThenBy(pi => pi.DisplayOrder)
            .FirstOrDefault();
        
        string? primaryImageUrl = null;
        if (primaryImage != null)
        {
            var extension = await _imageStorage.GetImageExtensionAsync(primaryImage.ImageId, cancellationToken);
            primaryImageUrl = extension != null 
                ? $"/api/images/{primaryImage.ImageId}{extension}" 
                : $"/api/images/{primaryImage.ImageId}";
        }

        // Variants-ı manual set et (Product məlumatları ilə)
        var variants = new List<ProductVariantDto>();
        foreach (var pv in product.ProductVariants)
        {
            var attributes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(pv.AttributesJson);
            string? imageUrl = null;
            if (pv.ImageId.HasValue)
            {
                var variantExtension = await _imageStorage.GetImageExtensionAsync(pv.ImageId.Value, cancellationToken);
                imageUrl = variantExtension != null 
                    ? $"/api/images/{pv.ImageId.Value}{variantExtension}" 
                    : $"/api/images/{pv.ImageId.Value}";
            }
            
            variants.Add(new ProductVariantDto
            {
                Id = pv.Id,
                Sku = product.Sku.Value, // Product-dan
                Price = product.Price.Amount, // Product-dan
                Currency = product.Price.Currency, // Product-dan
                Stock = product.Stock, // Product-dan
                IsActive = pv.IsActive,
                ImageId = pv.ImageId,
                ImageUrl = imageUrl,
                Attributes = attributes ?? new Dictionary<string, string>()
            });
        }

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

        // Variantlar üçün endirim hesabla (hamısı eyni Product-dan gəlir)
        var variantsWithDiscounts = variants.Select(variant => variant with
        {
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice
        }).ToList();

        var currentUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null;
        var ratingSummary = await _ratingRepository.GetSummaryAsync(product.Id, currentUserId, cancellationToken);

        // Endirim məlumatlarını və kateqoriya atributlarını DTO-ya əlavə et
        productDto = productDto with
        {
            PrimaryImageUrl = primaryImageUrl,
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice,
            AverageRating = ratingSummary.AverageRating,
            RatingCount = ratingSummary.RatingCount,
            CurrentUserRating = ratingSummary.CurrentUserRating,
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





