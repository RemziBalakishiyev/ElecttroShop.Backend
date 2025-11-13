using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProductById;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
{
    private readonly IProductQueryRepository _productRepository;

    public GetProductByIdQueryHandler(IProductQueryRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductDto>> Handle(
        GetProductByIdQuery request,
        CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(request.Id, cancellationToken);

        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.Id);
        }

        var productDto = product.Adapt<ProductDto>();

        return Result.Success(productDto);
    }
}





