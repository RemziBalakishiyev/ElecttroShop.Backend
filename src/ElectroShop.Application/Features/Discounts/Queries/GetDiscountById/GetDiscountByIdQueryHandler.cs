using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Queries.GetDiscountById;

public class GetDiscountByIdQueryHandler : IRequestHandler<GetDiscountByIdQuery, Result<DiscountDto>>
{
    private readonly IDiscountQueryRepository _discountRepository;

    public GetDiscountByIdQueryHandler(IDiscountQueryRepository discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<Result<DiscountDto>> Handle(
        GetDiscountByIdQuery request,
        CancellationToken cancellationToken)
    {
        var discount = await _discountRepository.GetDiscountWithDetailsAsync(
            request.Id, cancellationToken);

        if (discount == null)
        {
            return Result.Failure<DiscountDto>(
                Error.NotFound("Discount.NotFound", "Endirim tapılmadı"));
        }

        var discountDto = MapToDto(discount);
        return Result.Success(discountDto);
    }

    private static DiscountDto MapToDto(Discount discount)
    {
        return new DiscountDto
        {
            Id = discount.Id,
            Type = discount.Type,
            ProductId = discount.ProductId,
            ProductName = discount.Product?.Name,
            BrandId = discount.BrandId,
            BrandName = discount.Brand?.Name,
            CategoryId = discount.CategoryId,
            CategoryName = discount.Category?.Name,
            Percent = discount.Percent,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsActive = discount.IsActive,
            CreatedAt = discount.CreatedAtUtc,
            UpdatedAt = discount.UpdatedAtUtc
        };
    }
}

