using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.UpdateDiscount;

public class UpdateDiscountCommandHandler : IRequestHandler<UpdateDiscountCommand, Result<DiscountDto>>
{
    private readonly IWriteRepository<Discount> _discountWriteRepository;
    private readonly IDiscountQueryRepository _discountQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDiscountCommandHandler(
        IWriteRepository<Discount> discountWriteRepository,
        IDiscountQueryRepository discountQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _discountWriteRepository = discountWriteRepository;
        _discountQueryRepository = discountQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DiscountDto>> Handle(
        UpdateDiscountCommand request,
        CancellationToken cancellationToken)
    {
        var discount = await _discountQueryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (discount == null)
        {
            return Result.Failure<DiscountDto>(
                Error.NotFound("Discount.NotFound", "Endirim tapılmadı"));
        }

        // Endirimi yenilə
        discount.Update(request.Percent, request.StartDate, request.EndDate);

        // IsActive dəyişikliyi
        if (request.IsActive.HasValue)
        {
            if (request.IsActive.Value)
                discount.Activate();
            else
                discount.Deactivate();
        }

        _discountWriteRepository.Update(discount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Navigation properties ilə yenidən yüklə
        var updatedDiscount = await _discountQueryRepository.GetDiscountWithDetailsAsync(
            discount.Id, cancellationToken);

        if (updatedDiscount == null)
        {
            return Result.Failure<DiscountDto>(
                Error.NotFound("Discount.NotFound", "Yenilənmiş endirim tapılmadı"));
        }

        var discountDto = MapToDto(updatedDiscount);
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

