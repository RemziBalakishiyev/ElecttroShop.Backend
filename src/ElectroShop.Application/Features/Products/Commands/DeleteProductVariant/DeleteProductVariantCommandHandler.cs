using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Exceptions;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProductVariant;

/// <summary>
/// DeleteProductVariantCommandHandler - DDD Aggregate pattern
/// Variant yalnız Product aggregate vasitəsilə silinir (deaktiv edilir)
/// </summary>
public class DeleteProductVariantCommandHandler 
    : IRequestHandler<DeleteProductVariantCommand, Result>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteProductVariantCommandHandler(
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteProductVariantCommand request,
        CancellationToken cancellationToken)
    {
        // Product aggregate load (tracked) - variantlar daxil olmaqla
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);
        
        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        // Aggregate metod vasitəsilə variant sil (deaktiv et)
        try
        {
            product.RemoveVariant(request.VariantId);
        }
        catch (InvalidOperationException)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.VariantId));
        }

        // Tracked entity üçün Update() çağırmaq QADAĞANDIR
        try
        {
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        catch (ConcurrencyException ex)
        {
            return Result.Failure(
                Error.Conflict(
                    "Product.ConcurrencyConflict",
                    ex.Message
                ));
        }

        return Result.Success();
    }
}

