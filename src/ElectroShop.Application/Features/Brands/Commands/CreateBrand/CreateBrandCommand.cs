using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.CreateBrand;

public record CreateBrandCommand(string Name) : IRequest<Result<BrandDto>>;

