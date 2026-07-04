using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Common;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.UpdateSale;

public class UpdateSaleCommandHandler : IRequestHandler<UpdateSaleCommand, Result<SaleDetailDto>>
{
    private readonly ISaleQueryRepository _saleQueryRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateSaleCommandHandler(
        ISaleQueryRepository saleQueryRepository,
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _saleQueryRepository = saleQueryRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SaleDetailDto>> Handle(
        UpdateSaleCommand request,
        CancellationToken cancellationToken)
    {
        var sale = await _saleQueryRepository.GetSaleWithExpensesForUpdateAsync(request.Id, cancellationToken);
        if (sale is null)
            return Result.Failure<SaleDetailDto>(DomainErrors.Sale.NotFound(request.Id));

        var soldAt = request.SoldAt ?? sale.SoldAt;
        var previousQuantity = sale.Quantity;

        try
        {
            if (sale.SaleSource == SaleSource.ExistingProduct)
            {
                if (!sale.ProductId.HasValue)
                {
                    return Result.Failure<SaleDetailDto>(
                        Error.Failure("Sale.InvalidState", "Mövcud məhsul satışında ProductId tapılmadı"));
                }

                var product = await _productQueryRepository.GetByIdAsync(sale.ProductId.Value, cancellationToken);
                if (product is null)
                    return Result.Failure<SaleDetailDto>(DomainErrors.Product.NotFound(sale.ProductId.Value));

                var quantityDelta = request.Quantity - previousQuantity;
                if (quantityDelta > 0 && product.Stock < quantityDelta)
                    return Result.Failure<SaleDetailDto>(DomainErrors.Product.OutOfStock);

                sale.UpdateExistingProductSale(request.SalePrice, request.Quantity, soldAt, request.Note);

                if (quantityDelta > 0)
                {
                    product.DecreaseStock(quantityDelta);
                    if (product.Stock == 0)
                        product.Deactivate();
                }
                else if (quantityDelta < 0)
                {
                    product.IncreaseStock(-quantityDelta);
                    if (product.Stock > 0 && !product.IsActive)
                        product.Activate();
                }
            }
            else
            {
                if (!request.CostPrice.HasValue)
                {
                    return Result.Failure<SaleDetailDto>(
                        Error.Validation("Sale.CostPriceRequired", "Manual satış üçün maya dəyəri tələb olunur"));
                }

                sale.UpdateManualEntry(
                    request.ProductName ?? sale.ProductName,
                    request.ProductCode ?? sale.ProductCode,
                    request.CategoryId ?? sale.CategoryId,
                    request.CategoryName ?? sale.CategoryName,
                    request.CostPrice.Value,
                    request.SalePrice,
                    request.Quantity,
                    soldAt,
                    request.Note);
            }
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<SaleDetailDto>(Error.Validation("Sale.InvalidData", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<SaleDetailDto>(Error.Failure("Sale.InvalidOperation", ex.Message));
        }

        try
        {
            if (request.Expenses is not null)
            {
                var expenseDrafts = SaleMapper.ToExpenseDrafts(request.Expenses);
                sale.ReplaceExpenses(expenseDrafts);
            }
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<SaleDetailDto>(Error.Validation("Sale.InvalidExpense", ex.Message));
        }

        // Sale və Product artıq DbContext tərəfindən izlənilir (tracked).
        // Yeni expense-lər bəzən Modified state-ə düşür; SaveChanges-dən əvvəl düzəldilir.
        await _unitOfWork.PrepareSaleForSaveAsync(sale.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(SaleMapper.ToDetailDto(sale));
    }
}
