using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.CreateDiscount;

public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, Result<DiscountDto>>
{
    private readonly IWriteRepository<Discount> _discountWriteRepository;
    private readonly IDiscountQueryRepository _discountQueryRepository;
    private readonly IQueryRepository<Product> _productRepository;
    private readonly IQueryRepository<Brand> _brandRepository;
    private readonly IQueryRepository<Category> _categoryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateDiscountCommandHandler(
        IWriteRepository<Discount> discountWriteRepository,
        IDiscountQueryRepository discountQueryRepository,
        IQueryRepository<Product> productRepository,
        IQueryRepository<Brand> brandRepository,
        IQueryRepository<Category> categoryRepository,
        IUnitOfWork unitOfWork)
    {
        _discountWriteRepository = discountWriteRepository;
        _discountQueryRepository = discountQueryRepository;
        _productRepository = productRepository;
        _brandRepository = brandRepository;
        _categoryRepository = categoryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DiscountDto>> Handle(
        CreateDiscountCommand request,
        CancellationToken cancellationToken)
    {
        // Validation: Type-ə görə müvafiq ID-nin olması lazımdır
        if (request.Type == DiscountType.Product && !request.ProductId.HasValue)
        {
            return Result.Failure<DiscountDto>(
                Error.Validation("Discount.ProductId.Required", "Məhsul endirimi üçün məhsul ID-si tələb olunur"));
        }

        if (request.Type == DiscountType.Brand && !request.BrandId.HasValue)
        {
            return Result.Failure<DiscountDto>(
                Error.Validation("Discount.BrandId.Required", "Brend endirimi üçün brend ID-si tələb olunur"));
        }

        if (request.Type == DiscountType.Category && !request.CategoryId.HasValue)
        {
            return Result.Failure<DiscountDto>(
                Error.Validation("Discount.CategoryId.Required", "Kateqoriya endirimi üçün kateqoriya ID-si tələb olunur"));
        }

        // Entity-nin mövcud olub-olmadığını yoxla
        if (request.Type == DiscountType.Product && request.ProductId.HasValue)
        {
            var product = await _productRepository.GetByIdAsync(request.ProductId.Value, cancellationToken);
            if (product == null)
            {
                return Result.Failure<DiscountDto>(
                    Error.NotFound("Product.NotFound", "Məhsul tapılmadı"));
            }
        }

        if (request.Type == DiscountType.Brand && request.BrandId.HasValue)
        {
            var brand = await _brandRepository.GetByIdAsync(request.BrandId.Value, cancellationToken);
            if (brand == null)
            {
                return Result.Failure<DiscountDto>(
                    Error.NotFound("Brand.NotFound", "Brend tapılmadı"));
            }
        }

        if (request.Type == DiscountType.Category && request.CategoryId.HasValue)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category == null)
            {
                return Result.Failure<DiscountDto>(
                    Error.NotFound("Category.NotFound", "Kateqoriya tapılmadı"));
            }
        }

        // Discount yarat
        Discount discount = request.Type switch
        {
            DiscountType.Product => Discount.CreateProductDiscount(
                request.ProductId!.Value,
                request.Percent,
                request.StartDate,
                request.EndDate,
                request.IsActive),
            DiscountType.Brand => Discount.CreateBrandDiscount(
                request.BrandId!.Value,
                request.Percent,
                request.StartDate,
                request.EndDate,
                request.IsActive),
            DiscountType.Category => Discount.CreateCategoryDiscount(
                request.CategoryId!.Value,
                request.Percent,
                request.StartDate,
                request.EndDate,
                request.IsActive),
            _ => throw new ArgumentException("Dəstəklənməyən endirim tipi", nameof(request.Type))
        };

        await _discountWriteRepository.AddAsync(discount, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Discount-u yenidən yüklə (navigation properties üçün)
        var loadedDiscount = await _discountQueryRepository.GetDiscountWithDetailsAsync(
            discount.Id, cancellationToken);

        if (loadedDiscount == null)
        {
            return Result.Failure<DiscountDto>(
                Error.NotFound("Discount.NotFound", "Yaradılmış endirim tapılmadı"));
        }

        var discountDto = MapToDto(loadedDiscount);
        return Result.Success(discountDto);
    }

    private static DiscountDto MapToDto(Discount discount)
    {
        return new DiscountDto
        {
            Id = discount.Id,
            Type = discount.Type,
            ProductId = discount.ProductId,
            ProductName = discount.Product?.Name,
            BrandId = discount.BrandId,
            BrandName = discount.Brand?.Name,
            CategoryId = discount.CategoryId,
            CategoryName = discount.Category?.Name,
            Percent = discount.Percent,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsActive = discount.IsActive,
            CreatedAt = discount.CreatedAtUtc,
            UpdatedAt = discount.UpdatedAtUtc
        };
    }
}

