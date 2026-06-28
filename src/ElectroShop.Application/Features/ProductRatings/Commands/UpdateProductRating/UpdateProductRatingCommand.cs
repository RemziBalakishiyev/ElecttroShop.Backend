using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.UpdateProductRating;

public record UpdateProductRatingCommand(
    Guid ProductId,
    int RatingValue,
    string? Comment) : IRequest<Result<ProductRatingResponse>>;
