using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Brands.Commands.DeleteBrand;

public record DeleteBrandCommand(Guid Id) : IRequest<Result>;

