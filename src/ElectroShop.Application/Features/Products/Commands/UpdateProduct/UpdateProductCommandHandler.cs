using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// UpdateProductCommand üçün Handler
/// DDD və Clean Architecture prinsiplərinə uyğun
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IWriteRepository<ProductVariant> _variantRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IWriteRepository<Product> productRepository,
        IWriteRepository<ProductVariant> variantRepository,
        IProductQueryRepository productQueryRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _variantRepository = variantRepository;
        _productQueryRepository = productQueryRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        // Bütün validasiyalar FluentValidation tərəfindən edilib

        // Məhsulu tap (şəkillər və variantlarla birlikdə)
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.Id, 
            cancellationToken);
        
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        // Navigation entities-ləri götür
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        // Domain method istifadə edərək yenilə
        try
        {
            product.Update(
                name: request.Name,
                description: request.Description,
                price: request.Price,
                currency: request.Currency,
                categoryId: request.CategoryId,
                brandId: request.BrandId,
                vatRate: request.VatRate,
                stock: request.Stock
            );
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<ProductDto>(
                Error.Validation("Product.InvalidData", ex.Message));
        }

        // Şəkilləri yenilə
        // Köhnə şəkilləri sil
        var existingImageIds = product.ProductImages.Select(pi => pi.ImageId).ToList();
        var imagesToRemove = product.ProductImages
            .Where(pi => !request.ImageIds.Contains(pi.ImageId))
            .ToList();
        
        // Şəkilləri məhsuldan sil (domain method istifadə et)
        foreach (var image in imagesToRemove)
        {
            product.RemoveImage(image.ImageId);
        }

        // Yeni şəkilləri əlavə et
        var newImageIds = request.ImageIds.Where(id => !existingImageIds.Contains(id)).ToList();
        var currentMaxOrder = product.ProductImages.Any() 
            ? product.ProductImages.Max(pi => pi.DisplayOrder) + 1 
            : 0;

        foreach (var imageId in newImageIds)
        {
            var isPrimary = !product.ProductImages.Any(pi => pi.IsPrimary) && imageId == newImageIds.First();
            product.AddImage(imageId, currentMaxOrder++, isPrimary);
        }

        // Şəkil sırasını yenilə
        for (int i = 0; i < request.ImageIds.Count; i++)
        {
            var image = product.ProductImages.FirstOrDefault(pi => pi.ImageId == request.ImageIds[i]);
            if (image != null)
            {
                image.UpdateDisplayOrder(i);
                if (i == 0 && !image.IsPrimary)
                {
                    product.SetPrimaryImage(image.ImageId);
                }
            }
        }

        // Variantları yenilə
        var existingVariantIds = product.ProductVariants.Select(pv => pv.Id).ToList();
        
        // Yeni variantları əlavə et
        foreach (var variantDto in request.Variants.Where(v => !v.Id.HasValue))
        {
            var attributesJson = JsonSerializer.Serialize(variantDto.Attributes);
            var variant = ProductVariant.Create(
                product.Id,
                variantDto.Sku,
                variantDto.Price,
                variantDto.Currency,
                variantDto.Stock,
                attributesJson,
                variantDto.ImageId
            );
            if (!variantDto.IsActive)
            {
                variant.Deactivate();
            }
            await _variantRepository.AddAsync(variant, cancellationToken);
        }

        // Mövcud variantları yenilə
        foreach (var variantDto in request.Variants.Where(v => v.Id.HasValue))
        {
            var variant = product.ProductVariants.FirstOrDefault(pv => pv.Id == variantDto.Id!.Value);
            if (variant != null)
            {
                var attributesJson = JsonSerializer.Serialize(variantDto.Attributes);
                variant.Update(
                    variantDto.Sku,
                    variantDto.Price,
                    variantDto.Currency,
                    variantDto.Stock,
                    attributesJson,
                    variantDto.ImageId
                );
                if (variantDto.IsActive && !variant.IsActive)
                {
                    variant.Activate();
                }
                else if (!variantDto.IsActive && variant.IsActive)
                {
                    variant.Deactivate();
                }
                _variantRepository.Update(variant);
            }
        }

        // Silinmiş variantları deaktiv et
        var variantIdsToKeep = request.Variants
            .Where(v => v.Id.HasValue)
            .Select(v => v.Id!.Value)
            .ToList();
        var variantsToDeactivate = product.ProductVariants
            .Where(pv => !variantIdsToKeep.Contains(pv.Id))
            .ToList();
        
        foreach (var variant in variantsToDeactivate)
        {
            variant.Deactivate();
            _variantRepository.Update(variant);
        }

        // Dəyişiklikləri saxla
        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // DTO yarat
        var productDto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Sku = product.Sku.Value,
            CategoryId = product.CategoryId,
            CategoryName = category!.Name,
            BrandId = product.BrandId,
            BrandName = brand!.Name,
            VatRate = product.VatRate,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAtUtc,
            UpdatedAt = product.UpdatedAtUtc
        };

        return Result.Success(productDto);
    }
}

