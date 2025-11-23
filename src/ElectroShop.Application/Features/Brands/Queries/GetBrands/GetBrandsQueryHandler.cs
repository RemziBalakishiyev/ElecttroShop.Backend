using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrands;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, PagedResult<BrandDto>>
{
    private readonly IBrandQueryRepository _brandRepository;
    private readonly IDiscountCalculationService _discountCalculationService;

    public GetBrandsQueryHandler(
        IBrandQueryRepository brandRepository,
        IDiscountCalculationService discountCalculationService)
    {
        _brandRepository = brandRepository;
        _discountCalculationService = discountCalculationService;
    }

    public async Task<PagedResult<BrandDto>> Handle(
        GetBrandsQuery request,
        CancellationToken cancellationToken)
    {
        var (brands, totalCount) = await _brandRepository.GetBrandsPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<BrandDto>.Empty(request.Page, request.PageSize);
        }

        var brandDtos = new List<BrandDto>();

        // Hər brend üçün endirim faizini hesabla
        foreach (var brand in brands)
        {
            var discountPercent = await _discountCalculationService.GetBrandDiscountPercentAsync(
                brand.Id,
                cancellationToken);

            brandDtos.Add(new BrandDto
            {
                Id = brand.Id,
                Name = brand.Name,
                DiscountPercent = discountPercent,
                CreatedAt = brand.CreatedAtUtc
            });
        }

        return PagedResult<BrandDto>.Success(brandDtos, request.Page, request.PageSize, totalCount);
    }
}

