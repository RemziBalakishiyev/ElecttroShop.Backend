using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProductVariant;

/// <summary>
/// UpdateProductVariantCommandHandler - DDD Aggregate pattern
/// Variant yalnız Product aggregate vasitəsilə yenilənir
/// </summary>
public class UpdateProductVariantCommandHandler 
    : IRequestHandler<UpdateProductVariantCommand, Result<ProductVariantDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IDiscountCalculationService _discountCalculationService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductVariantCommandHandler(
        IProductQueryRepository productQueryRepository,
        IDiscountCalculationService discountCalculationService,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _discountCalculationService = discountCalculationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductVariantDto>> Handle(
        UpdateProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        // Product aggregate load (tracked) - variantlar daxil olmaqla
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);
        
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        // Variantın mövcud olduğunu yoxla
        var variant = product.ProductVariants.FirstOrDefault(v => v.Id == request.Id);
        if (variant is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        // Aggregate metod vasitəsilə variant yenilə
        var attributesJson = JsonSerializer.Serialize(request.Attributes);
        product.UpdateVariant(
            request.Id,
            attributesJson,
            request.ImageId,
            request.IsActive
        );

        // Tracked entity üçün Update() çağırmaq QADAĞANDIR
        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException ex)
        {
            return Result.Failure<ProductVariantDto>(
                Error.Conflict(
                    "Product.ConcurrencyConflict",
                    ex.Message
                ));
        }

        // Variantı yenidən tap (update-dən sonra)
        variant = product.ProductVariants.FirstOrDefault(v => v.Id == request.Id);
        if (variant == null)
        {
            return Result.Failure<ProductVariantDto>(
                DomainErrors.Product.NotFound(request.Id));
        }

        // Endirim hesabla (Product-dan)
        var discountPercent = await _discountCalculationService.CalculateFinalDiscountPercentAsync(
            product.Id,
            product.CategoryId,
            product.BrandId,
            cancellationToken);

        var finalPrice = _discountCalculationService.CalculateDiscountedPrice(
            product.Price.Amount,
            discountPercent);

        var variantDto = new ProductVariantDto
        {
            Id = variant.Id,
            Sku = product.Sku.Value, // Product-dan
            Price = product.Price.Amount, // Product-dan
            Currency = product.Price.Currency, // Product-dan
            Stock = product.Stock, // Product-dan
            IsActive = variant.IsActive,
            ImageId = variant.ImageId,
            ImageUrl = variant.ImageId.HasValue ? $"/api/images/{variant.ImageId}" : null,
            Attributes = request.Attributes,
            FinalDiscountPercent = discountPercent,
            FinalPrice = finalPrice
        };

        return Result.Success(variantDto);
    }
}



