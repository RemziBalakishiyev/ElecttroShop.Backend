using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.AddProductImage;

public class AddProductImageCommandHandler 
    : IRequestHandler<AddProductImageCommand, Result<ProductImageDto>>
{
    private readonly IWriteRepository<Product> _productRepository;
    private readonly IQueryRepository<Product> _productQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddProductImageCommandHandler(
        IWriteRepository<Product> productRepository,
        IQueryRepository<Product> productQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _productQueryRepository = productQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ProductImageDto>> Handle(
        AddProductImageCommand request,
        CancellationToken cancellationToken)
    {
        var product = await _productQueryRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        product.AddImage(request.ImageId, request.DisplayOrder, request.IsPrimary);

        _productRepository.Update(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Yeni yaradılmış şəkili tap
        var productImage = product.ProductImages.FirstOrDefault(pi => pi.ImageId == request.ImageId);
        if (productImage is null)
        {
            return Result.Failure<ProductImageDto>(Error.Failure("ProductImage.NotFound", "Şəkil tapılmadı"));
        }

        var imageDto = new ProductImageDto
        {
            Id = productImage.Id,
            ImageId = productImage.ImageId,
            ImageUrl = $"/api/images/{productImage.ImageId}",
            DisplayOrder = productImage.DisplayOrder,
            IsPrimary = productImage.IsPrimary
        };

        return Result.Success(imageDto);
    }
}



