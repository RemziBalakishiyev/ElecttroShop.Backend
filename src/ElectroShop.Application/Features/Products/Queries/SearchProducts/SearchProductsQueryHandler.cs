using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryHandler : IRequestHandler<SearchProductsQuery, PagedResult<ProductListDto>>
{
    private readonly IProductQueryRepository _productRepository;

    public SearchProductsQueryHandler(IProductQueryRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListDto>> Handle(
        SearchProductsQuery request,
        CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetProductsPagedAsync(
            request.Page,
            request.PageSize,
            searchTerm: request.SearchTerm,
            cancellationToken: cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<ProductListDto>.Empty(request.Page, request.PageSize);
        }

        var productDtos = products.Adapt<List<ProductListDto>>();

        return PagedResult<ProductListDto>.Success(productDtos, request.Page, request.PageSize, totalCount);
    }
}

