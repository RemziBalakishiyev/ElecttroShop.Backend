using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Products.Queries.GetProductById;

/// <summary>
/// ID-yə görə məhsul əldə etmək üçün Query
/// Result pattern ilə ProductDto qaytarır
/// </summary>
public record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;

