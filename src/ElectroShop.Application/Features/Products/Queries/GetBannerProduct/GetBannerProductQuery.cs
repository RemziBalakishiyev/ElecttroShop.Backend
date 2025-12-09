using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetBannerProduct;

public record GetBannerProductQuery() : IRequest<Result<ProductDto>>;



