using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Exceptions;
using MediatR;
using System.Text.Json;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// UpdateProductCommandHandler - DDD və EF Core Optimistic Concurrency best practices
/// 
/// PRINCIPLES:
/// 1. Product aggregate root kimi davranır
/// 2. Child entity-lər (ProductVariant, ProductImage) yalnız aggregate metodları vasitəsilə dəyişdirilir
/// 3. Tracked entity üçün Update() çağırılmır - ChangeTracker avtomatik işləyir
/// 4. RowVersion manual yoxlanılır (PostgreSQL xmin SaveChanges-dən sonra yenilənir)
/// 5. DbUpdateConcurrencyException xüsusi olaraq tutulur
/// </summary>
public class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductQueryRepository productQueryRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductDto>> Handle(
        UpdateProductCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository
            .GetProductWithImagesAndVariantsAsync(request.Id, cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound(request.Id);

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        // RowVersion client-dən yalnız informasiya üçündür — /images kimi aralıq
        // API çağırışları xmin-i dəyişir; manual pre-check false-positive 409 yaradır.
        // Real concurrency SaveChanges zamanı EF xmin ilə yoxlanılır.

        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        var brand = await _brandRepository.GetByIdAsync(request.BrandId, cancellationToken);

        if (category is null || brand is null)
            return Result.Failure<ProductDto>(
                Error.Validation("Product.InvalidRelation", "Category or Brand not found"));

        product.UpdateDetails(
            name: request.Name,
            description: request.Description,
            price: request.Price,
            currency: request.Currency,
            categoryId: request.CategoryId,
            brandId: request.BrandId,
            vatRate: request.VatRate,
            stock: request.Stock
        );

        product.SyncImages(request.ImageIds);

        var variantData = request.Variants.Select(v => (
            Id: v.Id,
            AttributesJson: JsonSerializer.Serialize(v.Attributes),
            ImageId: v.ImageId,
            IsActive: v.IsActive
        )).ToList();

        product.SyncVariants(variantData);

        var activeVariantIds = request.Variants
            .Where(v => v.Id.HasValue)
            .Select(v => v.Id!.Value)
            .ToList();

        product.DeactivateMissingVariants(activeVariantIds);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);

        try
        {
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException)
        {
                return Result.Failure<ProductDto>(
                Error.Conflict( 
                    "Product.ConcurrencyConflict",
                    "The data has been modified by another user. Please reload and try again."
                ));
        }

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
            UpdatedAt = product.UpdatedAtUtc,
            RowVersion = product.RowVersion
        };

        return Result.Success(dto);
    }
}
