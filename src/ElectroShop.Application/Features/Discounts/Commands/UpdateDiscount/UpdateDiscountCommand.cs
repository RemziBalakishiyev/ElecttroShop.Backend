using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.UpdateDiscount;

public record UpdateDiscountCommand(
    Guid Id,
    decimal Percent,
    DateTime StartDate,
    DateTime? EndDate = null,
    bool? IsActive = null) : IRequest<Result<DiscountDto>>;









