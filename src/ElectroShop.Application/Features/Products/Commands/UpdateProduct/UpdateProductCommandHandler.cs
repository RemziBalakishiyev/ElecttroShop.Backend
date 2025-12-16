using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
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
        // 1️⃣ Product load (tracked)
        var product = await _productQueryRepository
            .GetProductWithImagesAndVariantsAsync(request.Id, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound(request.Id);

        // 2️⃣ Navigation entities
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (category is null || brand is null)
            return Result.Failure<ProductDto>(
                Error.Validation("Product.InvalidRelation", "Category or Brand not found"));

        // 3️⃣ Core product update (Domain method)
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

        // ===================== IMAGES (SAFE DIFF UPDATE) =====================

        var existingImageIds = product.ProductImages
            .Select(x => x.ImageId)
            .ToList();

        // Remove
        var imagesToRemove = product.ProductImages
            .Where(x => !request.ImageIds.Contains(x.ImageId))
            .ToList();

        foreach (var image in imagesToRemove)
        {
            product.RemoveImage(image.ImageId);
        }

        // Add
        foreach (var imageId in request.ImageIds)
        {
            if (!existingImageIds.Contains(imageId))
            {
                var order = request.ImageIds.IndexOf(imageId);
                var isPrimary = order == 0;
                product.AddImage(imageId, order, isPrimary);
            }
        }

        // ===================== VARIANTS =====================

        // Add new variants
        foreach (var variantDto in request.Variants.Where(v => !v.Id.HasValue))
        {
            var attributesJson = JsonSerializer.Serialize(variantDto.Attributes);

            var variant = ProductVariant.Create(
                product.Id,
                attributesJson,
                variantDto.ImageId
            );

            if (!variantDto.IsActive)
                variant.Deactivate();

            await _variantRepository.AddAsync(variant, cancellationToken);
        }

        // Update existing variants
        foreach (var variantDto in request.Variants.Where(v => v.Id.HasValue))
        {
            var variant = product.ProductVariants
                .FirstOrDefault(x => x.Id == variantDto.Id!.Value);

            if (variant is null)
                continue;

            var attributesJson = JsonSerializer.Serialize(variantDto.Attributes);

            variant.Update(attributesJson, variantDto.ImageId);

            if (variantDto.IsActive)
                variant.Activate();
            else
                variant.Deactivate();

            _variantRepository.Update(variant);
        }

        // Deactivate removed variants
        var activeVariantIds = request.Variants
            .Where(v => v.Id.HasValue)
            .Select(v => v.Id!.Value)
            .ToList();

        foreach (var variant in product.ProductVariants
                     .Where(v => !activeVariantIds.Contains(v.Id)))
        {
            variant.Deactivate();
            _variantRepository.Update(variant);
        }

        // ===================== SAVE (CONCURRENCY SAFE) =====================

        try
        {
            _productRepository.Update(product);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (Exception)
        {
            return Result.Failure<ProductDto>(
                Error.Conflict(
                    "Product.ConcurrencyError",
                    "Məhsul başqa istifadəçi tərəfindən dəyişdirilib. Yenidən cəhd edin."
                ));
        }

        // ===================== DTO =====================

        var dto = new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price.Amount,
            Currency = product.Price.Currency,
            Sku = product.Sku.Value,
            CategoryId = product.CategoryId,
            CategoryName = category.Name,
            BrandId = product.BrandId,
            BrandName = brand.Name,
            VatRate = product.VatRate,
            Stock = product.Stock,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAtUtc,
            UpdatedAt = product.UpdatedAtUtc
        };

        return Result.Success(dto);
    }
}
