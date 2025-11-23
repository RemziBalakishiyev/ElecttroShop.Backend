using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.CreateDiscount;

public record CreateDiscountCommand(
    DiscountType Type,
    Guid? ProductId,
    Guid? BrandId,
    Guid? CategoryId,
    decimal Percent,
    DateTime StartDate,
    DateTime? EndDate = null,
    bool IsActive = true) : IRequest<Result<DiscountDto>>;

