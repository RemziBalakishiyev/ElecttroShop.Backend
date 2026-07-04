using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.AddProductImage;

public class AddProductImageCommandHandler
    : IRequestHandler<AddProductImageCommand, Result<ProductImageDto>>
{
    private readonly IProductQueryRepository _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IImageUrlResolver _imageUrlResolver;

    public AddProductImageCommandHandler(
        IProductQueryRepository productQueryRepository,
        IUnitOfWork unitOfWork,
        IImageUrlResolver imageUrlResolver)
    {
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<ProductImageDto>> Handle(
        AddProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetProductWithImagesAndVariantsAsync(
            request.ProductId,
            cancellationToken);

        if (product is null)
            return DomainErrors.Product.NotFound(request.ProductId);

        await _productQueryRepository.EnsureProductImagesAttachedAsync(product, cancellationToken);

        if (request.IsPrimary)
        {
            foreach (var image in product.ProductImages.Where(pi => pi.IsPrimary))
                image.RemoveAsPrimary();
        }

        product.AddImage(request.ImageId, request.DisplayOrder, request.IsPrimary);

        await _unitOfWork.PrepareProductAggregateForSaveAsync(product.Id, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var productImage = product.ProductImages.FirstOrDefault(pi => pi.ImageId == request.ImageId);
        if (productImage is null)
        {
            return Result.Failure<ProductImageDto>(
                Error.Failure("ProductImage.NotFound", "Şəkil tapılmadı"));
        }

        return Result.Success(new ProductImageDto
        {
            Id = productImage.Id,
            ImageId = productImage.ImageId,
            ImageUrl = await _imageUrlResolver.BuildImageUrlAsync(productImage.ImageId, cancellationToken),
            DisplayOrder = productImage.DisplayOrder,
            IsPrimary = productImage.IsPrimary
        });
    }
}
