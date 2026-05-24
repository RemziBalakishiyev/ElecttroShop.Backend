using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductImage;

public class RemoveProductImageCommandHandler 
    : IRequestHandler<RemoveProductImageCommand, Result>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProductImageCommandHandler(
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        RemoveProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        product.RemoveImage(request.ImageId);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

