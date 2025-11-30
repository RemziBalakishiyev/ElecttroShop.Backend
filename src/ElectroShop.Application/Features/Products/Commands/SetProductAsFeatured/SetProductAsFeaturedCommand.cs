using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsFeatured;

public record SetProductAsFeaturedCommand(Guid ProductId, int DisplayOrder) : IRequest<Result>;


