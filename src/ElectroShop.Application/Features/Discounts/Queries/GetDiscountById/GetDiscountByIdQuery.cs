using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Queries.GetDiscountById;

public record GetDiscountByIdQuery(Guid Id) : IRequest<Result<DiscountDto>>;









