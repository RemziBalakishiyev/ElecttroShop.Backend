using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.DeleteDiscount;

public record DeleteDiscountCommand(Guid Id) : IRequest<Result<bool>>;

