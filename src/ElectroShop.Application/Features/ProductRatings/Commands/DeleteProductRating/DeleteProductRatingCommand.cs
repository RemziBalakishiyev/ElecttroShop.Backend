using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Commands.DeleteProductRating;

public record DeleteProductRatingCommand(Guid ProductId) : IRequest<Result>;
