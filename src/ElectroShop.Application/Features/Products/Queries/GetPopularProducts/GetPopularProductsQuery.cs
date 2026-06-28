using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetPopularProducts;

public record GetPopularProductsQuery() : IRequest<Result<List<PopularProductDto>>>;
