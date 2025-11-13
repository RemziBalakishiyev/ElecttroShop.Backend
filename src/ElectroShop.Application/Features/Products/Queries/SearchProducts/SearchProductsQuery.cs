using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.SearchProducts;

public record SearchProductsQuery(
    string SearchTerm,
    int Page = 1,
    int PageSize = 20) : IRequest<PagedResult<ProductListDto>>;

