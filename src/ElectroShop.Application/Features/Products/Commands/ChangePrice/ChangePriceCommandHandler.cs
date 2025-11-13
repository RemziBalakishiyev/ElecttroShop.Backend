using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.ChangePrice;

public class ChangePriceCommandHandler : IRequestHandler<ChangePriceCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ChangePriceCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IWriteRepository<Product> productWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _productWriteRepository = productWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ChangePriceCommand request, CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        try
        {
            product.ChangePrice(request.NewPrice);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure(Error.Validation("Product.InvalidPrice", ex.Message));
        }

        _productWriteRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

