using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Services;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetPopularProducts;

public class GetPopularProductsQueryHandler : IRequestHandler<GetPopularProductsQuery, Result<List<PopularProductDto>>>
{
    private readonly IProductQueryRepository _productRepository;
    private readonly IImageUrlResolver _imageUrlResolver;

    public GetPopularProductsQueryHandler(
        IProductQueryRepository productRepository,
        IImageUrlResolver imageUrlResolver)
    {
        _productRepository = productRepository;
        _imageUrlResolver = imageUrlResolver;
    }

    public async Task<Result<List<PopularProductDto>>> Handle(
        GetPopularProductsQuery request,
        CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetPopularProductsAsync(cancellationToken);

        if (products.Count == 0)
        {
            return Result.Success(new List<PopularProductDto>());
        }

        var productDtos = new List<PopularProductDto>();

        foreach (var product in products)
        {
            var primaryImage = product.ProductImages
                .OrderBy(pi => pi.IsPrimary ? 0 : 1)
                .ThenBy(pi => pi.DisplayOrder)
                .FirstOrDefault();

            string? imageUrl = null;
            if (primaryImage != null)
            {
                imageUrl = await _imageUrlResolver.BuildImageUrlAsync(primaryImage.ImageId, cancellationToken);
            }

            productDtos.Add(new PopularProductDto
            {
                Id = product.Id,
                Name = product.Name,
                ShortDescription = product.Description,
                ImageUrl = imageUrl,
                DisplayOrder = product.PopularDisplayOrder
            });
        }

        return Result.Success(productDtos);
    }
}
