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
    private readonly IImageUrlResolver _imageUrlResolver;
    private readonly ICurrentUserService _currentUserService;

    public GetProductByIdQueryHandler(
        IProductQueryRepository productRepository,
        ICategoryQueryRepository categoryRepository,
        IProductRatingQueryRepository ratingRepository,
        IDiscountCalculationService discountCalculationService,
        IImageUrlResolver imageUrlResolver,
        ICurrentUserService currentUserService)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _ratingRepository = ratingRepository;
        _discountCalculationService = discountCalculationService;
        _imageUrlResolver = imageUrlResolver;
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
            primaryImageUrl = await _imageUrlResolver.ResolveProductImageUrlAsync(primaryImage, cancellationToken);
        }

        var images = new List<ProductImageDto>();
        foreach (var image in product.ProductImages.OrderBy(pi => pi.DisplayOrder))
        {
            images.Add(new ProductImageDto
            {
                Id = image.Id,
                ImageId = image.ImageId,
                ImageUrl = await _imageUrlResolver.ResolveProductImageUrlAsync(image, cancellationToken),
                DisplayOrder = image.DisplayOrder,
                IsPrimary = image.IsPrimary
            });
        }

        // Variants-ı manual set et (Product məlumatları ilə)
        var variants = new List<ProductVariantDto>();
        foreach (var pv in product.ProductVariants)
        {
            var attributes = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(pv.AttributesJson);
            string? imageUrl = null;
            if (pv.ImageId.HasValue)
            {
                imageUrl = await _imageUrlResolver.BuildImageUrlAsync(pv.ImageId.Value, cancellationToken);
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

        // Məhsulun öz atributları (spesifikasiyaları) - kateqoriyadan deyil, məhsuldan gəlir
        var productAttributes = product.ProductAttributes
            .OrderBy(pa => pa.DisplayOrder)
            .Select(pa => new CategoryAttributeDto
            {
                Id = pa.Id,
                Name = pa.Name,
                DisplayName = pa.DisplayName,
                AttributeType = pa.AttributeType,
                IsRequired = pa.IsRequired,
                DisplayOrder = pa.DisplayOrder,
                Values = pa.Values.Select(pav => new CategoryAttributeValueDto
                {
                    Id = pav.Id,
                    Value = pav.Value,
                    DisplayValue = pav.DisplayValue,
                    DisplayOrder = pav.DisplayOrder,
                    ColorCode = pav.ColorCode
                }).OrderBy(v => v.DisplayOrder).ToList()
            })
            .ToList();

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
            Images = images,
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice,
            AverageRating = ratingSummary.AverageRating,
            RatingCount = ratingSummary.RatingCount,
            CurrentUserRating = ratingSummary.CurrentUserRating,
            CategoryAttributes = productAttributes,
            Variants = variantsWithDiscounts
        };

        return Result.Success(productDto);
    }
}





