using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductImage;

public class RemoveProductImageCommandHandler 
    : IRequestHandler<RemoveProductImageCommand, Result>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProductImageCommandHandler(
        IWriteRepository<Product> productRepository,
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
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

        product.RemoveImage(request.ImageId);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

