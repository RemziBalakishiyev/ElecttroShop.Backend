using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.UpdateBrand;

public class UpdateBrandCommandHandler : IRequestHandler<UpdateBrandCommand, Result<BrandDto>>
{
    private readonly IWriteRepository<Brand> _brandRepository;
    private readonly IQueryRepository<Brand> _brandQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDiscountCalculationService _discountCalculationService;

    public UpdateBrandCommandHandler(
        IWriteRepository<Brand> brandRepository,
        IQueryRepository<Brand> brandQueryRepository,
        IUnitOfWork unitOfWork,
        IDiscountCalculationService discountCalculationService)
    {
        _brandRepository = brandRepository;
        _brandQueryRepository = brandQueryRepository;
        _unitOfWork = unitOfWork;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<BrandDto>> Handle(
        UpdateBrandCommand request,
        CancellationToken cancellationToken)
    {
        var brand = await _brandQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (brand is null)
        {
            return DomainErrors.Brand.NotFound(request.Id);
        }

        try
        {
            brand.Update(request.Name);
            
            // Promotional status-u yenilə
            if (request.IsPromotional.HasValue)
            {
                brand.SetPromotional(request.IsPromotional.Value, request.DisplayOrder);
            }
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<BrandDto>(Error.Validation("Brand.InvalidData", ex.Message));
        }

        _brandRepository.Update(brand);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Endirim faizini hesabla
        var discountPercent = await _discountCalculationService.GetBrandDiscountPercentAsync(
            brand.Id,
            cancellationToken);

        var brandDto = new BrandDto
        {
            Id = brand.Id,
            Name = brand.Name,
            DiscountPercent = discountPercent,
            IsPromotional = brand.IsPromotional,
            DisplayOrder = brand.DisplayOrder,
            CreatedAt = brand.CreatedAtUtc
        };

        return Result.Success(brandDto);
    }
}

