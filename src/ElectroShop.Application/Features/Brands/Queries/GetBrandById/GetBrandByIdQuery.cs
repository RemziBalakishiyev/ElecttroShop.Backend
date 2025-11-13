using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrandById;

public record GetBrandByIdQuery(Guid Id) : IRequest<Result<BrandDto>>;

