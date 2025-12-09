using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromFeatured;

public record RemoveProductFromFeaturedCommand(Guid ProductId) : IRequest<Result>;



