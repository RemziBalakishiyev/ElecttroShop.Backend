using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Discounts.Commands.DeleteDiscount;

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, Result<bool>>
{
    private readonly IQueryRepository<Discount> _discountQueryRepository;
    private readonly IWriteRepository<Discount> _discountWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDiscountCommandHandler(
        IQueryRepository<Discount> discountQueryRepository,
        IWriteRepository<Discount> discountWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _discountQueryRepository = discountQueryRepository;
        _discountWriteRepository = discountWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(
        DeleteDiscountCommand request,
        CancellationToken cancellationToken)
    {
        var discount = await _discountQueryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (discount == null)
        {
            return Result.Failure<bool>(
                Error.NotFound("Discount.NotFound", "Endirim tapılmadı"));
        }

        // Soft delete - deaktiv et
        discount.Deactivate();
        _discountWriteRepository.Update(discount);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }
}






