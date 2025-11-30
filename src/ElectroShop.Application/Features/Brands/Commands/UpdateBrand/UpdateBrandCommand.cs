using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.UpdateBrand;

public record UpdateBrandCommand(
    Guid Id, 
    string Name,
    bool? IsPromotional = null,
    int? DisplayOrder = null) : IRequest<Result<BrandDto>>;

