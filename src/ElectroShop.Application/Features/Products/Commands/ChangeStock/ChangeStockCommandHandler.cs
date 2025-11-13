using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.ChangeStock;

public class ChangeStockCommandHandler : IRequestHandler<ChangeStockCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangeStockCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangeStockCommand request, CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        try
        {
            if (request.Operation == StockOperation.Increase)
            {
                product.IncreaseStock(request.Quantity);
            }
            else
            {
                product.DecreaseStock(request.Quantity);
            }
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Product.InvalidStock", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure(Error.Failure("Product.StockOperation", ex.Message));
        }

        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

