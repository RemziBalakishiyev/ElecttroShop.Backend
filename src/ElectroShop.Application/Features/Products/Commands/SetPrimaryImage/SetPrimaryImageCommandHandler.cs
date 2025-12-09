using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetPrimaryImage;

public class SetPrimaryImageCommandHandler 
    : IRequestHandler<SetPrimaryImageCommand, Result>
{
    private readonly IWriteRepository<Domain.Entities.Product> _productRepository;
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SetPrimaryImageCommandHandler(
        IWriteRepository<Domain.Entities.Product> productRepository,
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SetPrimaryImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId, 
            cancellationToken);

        if (product is null)
        {
            return Result.Failure(DomainErrors.Product.NotFound(request.ProductId));
        }

        product.SetPrimaryImage(request.ImageId);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

