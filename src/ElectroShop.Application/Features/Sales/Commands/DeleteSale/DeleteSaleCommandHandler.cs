using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.DeleteSale;

public class DeleteSaleCommandHandler : IRequestHandler<DeleteSaleCommand, Result>
{
    private readonly ISaleQueryRepository _saleQueryRepository;
    private readonly IWriteRepository<Sale> _saleWriteRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteSaleCommandHandler(
        ISaleQueryRepository saleQueryRepository,
        IWriteRepository<Sale> saleWriteRepository,
        IProductQueryRepository productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _saleQueryRepository = saleQueryRepository;
        _saleWriteRepository = saleWriteRepository;
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await _saleQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale is null)
            return Result.Failure(DomainErrors.Sale.NotFound(request.Id));

        if (sale.SaleSource == SaleSource.ExistingProduct && sale.ProductId.HasValue)
        {
            var product = await _productQueryRepository.GetByIdAsync(sale.ProductId.Value, cancellationToken);
            if (product is not null)
            {
                try
                {
                    product.IncreaseStock(sale.Quantity);
                    if (product.Stock > 0 && !product.IsActive)
                        product.Activate();

                    _productWriteRepository.Update(product);
                }
                catch (ArgumentException ex)
                {
                    return Result.Failure(Error.Validation("Product.InvalidStock", ex.Message));
                }
            }
        }

        sale.MarkDeleted();
        _saleWriteRepository.Update(sale);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
