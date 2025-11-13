using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrands;

public record GetBrandsQuery(
    int Page = 1,
    int PageSize = 10,
    string? SearchTerm = null) : IRequest<PagedResult<BrandDto>>;

