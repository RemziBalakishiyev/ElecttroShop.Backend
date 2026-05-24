using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Queries.GetDiscounts;

public record GetDiscountsQuery(
    int Page = 1,
    int PageSize = 10,
    DiscountType? Type = null,
    bool? IsActive = null,
    string? SearchTerm = null) : IRequest<PagedResult<DiscountListDto>>;









