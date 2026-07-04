using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Features.Sales.Common;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.CreateSale;

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Result<SaleDetailDto>>
{
    private readonly IWriteRepository<Sale> _saleWriteRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateSaleCommandHandler(
        IWriteRepository<Sale> saleWriteRepository,
        IProductQueryRepository productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _saleWriteRepository = saleWriteRepository;
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<SaleDetailDto>> Handle(
        CreateSaleCommand request,
        CancellationToken cancellationToken)
    {
        var soldAt = request.SoldAt ?? DateTime.UtcNow;

        Sale sale;
        try
        {
            if (request.ProductId.HasValue)
            {
                var product = await _productQueryRepository.GetProductWithDetailsAsync(
                    request.ProductId.Value,
                    cancellationToken);

                if (product is null)
                    return Result.Failure<SaleDetailDto>(DomainErrors.Product.NotFound(request.ProductId.Value));

                if (product.Stock < request.Quantity)
                    return Result.Failure<SaleDetailDto>(DomainErrors.Product.OutOfStock);

                sale = Sale.CreateFromExistingProduct(
                    product.Id,
                    product.Name,
                    product.Sku.Value,
                    product.CategoryId,
                    product.Category?.Name,
                    product.Price.Amount,
                    request.SalePrice,
                    request.Quantity,
                    soldAt,
                    request.Note);

                product.DecreaseStock(request.Quantity);
                if (product.Stock == 0)
                    product.Deactivate();

                _productWriteRepository.Update(product);
            }
            else
            {
                if (!request.CostPrice.HasValue)
                {
                    return Result.Failure<SaleDetailDto>(
                        Error.Validation("Sale.CostPriceRequired", "Manual satış üçün maya dəyəri tələb olunur"));
                }

                sale = Sale.CreateManualEntry(
                    request.ProductName!,
                    request.ProductCode,
                    request.CategoryId,
                    request.CategoryName,
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
            var expenseDrafts = SaleMapper.ToExpenseDrafts(request.Expenses);
            if (expenseDrafts.Count > 0)
                sale.SetExpenses(expenseDrafts);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<SaleDetailDto>(Error.Validation("Sale.InvalidExpense", ex.Message));
        }

        await _saleWriteRepository.AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(SaleMapper.ToDetailDto(sale));
    }
}
