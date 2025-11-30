using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrandById;

public class GetBrandByIdQueryHandler : IRequestHandler<GetBrandByIdQuery, Result<BrandDto>>
{
    private readonly IQueryRepository<Domain.Entities.Brand> _brandRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetBrandByIdQueryHandler(
        IQueryRepository<Domain.Entities.Brand> brandRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _brandRepository = brandRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<Result<BrandDto>> Handle(
        GetBrandByIdQuery request,
        CancellationToken cancellationToken)
    {
        var brand = await _brandRepository.FirstOrDefaultAsync(
            b => b.Id == request.Id && !b.IsDeleted,
            cancellationToken);

        if (brand is null)
        {
            return DomainErrors.Brand.NotFound(request.Id);
        }

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

