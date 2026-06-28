using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetMyProductRating;

public record GetMyProductRatingQuery(Guid ProductId) : IRequest<Result<ProductRatingResponse>>;
