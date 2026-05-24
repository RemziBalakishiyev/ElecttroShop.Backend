using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// DeleteProductCommand üçün Handler
/// Soft delete - IsDeleted = true, DDD pattern
/// </summary>
public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductCommandHandler(
        IQueryRepository<Product> productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteProductCommand request,
        CancellationToken cancellationToken)
    {
        // Məhsulu tap
        var product = await _productQueryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.Id));
        }

        // Soft delete - BaseCommonEntity-dən gələn method
        product.MarkDeleted();

        // Məhsulu deaktiv et
        product.Deactivate();

        // Tracked entity üçün Update() çağırmaq QADAĞANDIR
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

