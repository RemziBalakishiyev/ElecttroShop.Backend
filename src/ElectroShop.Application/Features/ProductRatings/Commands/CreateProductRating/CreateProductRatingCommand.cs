using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.CreateProductRating;

public record CreateProductRatingCommand(
    Guid ProductId,
    int RatingValue,
    string? Comment) : IRequest<Result<ProductRatingResponse>>;
