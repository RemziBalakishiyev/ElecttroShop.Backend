using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IProductQueryRepository _productRepository;

    public GetProductsQueryHandler(IProductQueryRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetProductsPagedAsync(
            request.Page,
            request.PageSize,
            request.SearchTerm,
            request.CategoryId,
            request.BrandId,
            request.MinPrice,
            request.MaxPrice,
            request.IsActive,
            cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<ProductListDto>.Empty(request.Page, request.PageSize);
        }

        var productDtos = products.Adapt<List<ProductListDto>>();

        return PagedResult<ProductListDto>.Success(productDtos, request.Page, request.PageSize, totalCount);
    }
}







