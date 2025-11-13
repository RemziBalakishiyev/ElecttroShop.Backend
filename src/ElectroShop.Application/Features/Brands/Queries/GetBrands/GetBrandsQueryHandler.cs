using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrands;

public class GetBrandsQueryHandler : IRequestHandler<GetBrandsQuery, PagedResult<BrandDto>>
{
    private readonly IBrandQueryRepository _brandRepository;

    public GetBrandsQueryHandler(IBrandQueryRepository brandRepository)
    {
        _brandRepository = brandRepository;
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

        var brandDtos = brands.Adapt<List<BrandDto>>();

        return PagedResult<BrandDto>.Success(brandDtos, request.Page, request.PageSize, totalCount);
    }
}

