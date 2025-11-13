using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.UpdateProduct;

/// <summary>
/// UpdateProductCommand üçün Handler
/// DDD və Clean Architecture prinsiplərinə uyğun
/// </summary>
public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<ProductDto>>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IWriteRepository<Product> productRepository,
        IQueryRepository<Product> productQueryRepository,
        IQueryRepository<Category> categoryRepository,
        IQueryRepository<Brand> brandRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
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

        // Məhsulu tap
        var product = await _productQueryRepository.GetByIdAsync(request.Id, cancellationToken);
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

