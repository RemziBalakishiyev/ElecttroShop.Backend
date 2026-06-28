using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatingSummary;

public record GetProductRatingSummaryQuery(Guid ProductId) : IRequest<Result<ProductRatingSummaryResponse>>;
