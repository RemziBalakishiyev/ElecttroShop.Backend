using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Queries.GetDiscounts;

public class GetDiscountsQueryHandler : IRequestHandler<GetDiscountsQuery, PagedResult<DiscountListDto>>
{
    private readonly IDiscountQueryRepository _discountRepository;

    public GetDiscountsQueryHandler(IDiscountQueryRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<PagedResult<DiscountListDto>> Handle(
        GetDiscountsQuery request,
        CancellationToken cancellationToken)
    {
        var (discounts, totalCount) = await _discountRepository.GetDiscountsPagedAsync(
            request.Page,
            request.PageSize,
            request.Type,
            request.IsActive,
            request.SearchTerm,
            cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<DiscountListDto>.Empty(request.Page, request.PageSize);
        }

        var discountDtos = discounts.Select(MapToDto).ToList();

        return PagedResult<DiscountListDto>.Success(
            discountDtos,
            request.Page,
            request.PageSize,
            totalCount);
    }

    private static DiscountListDto MapToDto(Discount discount)
    {
        var targetName = discount.Type switch
        {
            DiscountType.Product => discount.Product?.Name ?? "Unknown",
            DiscountType.Brand => discount.Brand?.Name ?? "Unknown",
            DiscountType.Category => discount.Category?.Name ?? "Unknown",
            _ => "Unknown"
        };

        return new DiscountListDto
        {
            Id = discount.Id,
            Type = discount.Type,
            TargetName = targetName,
            Percent = discount.Percent,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsActive = discount.IsActive
        };
    }
}

