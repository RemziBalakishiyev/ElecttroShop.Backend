using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetFeaturedProducts;

public record GetFeaturedProductsQuery() : IRequest<Result<List<ProductListDto>>>;



