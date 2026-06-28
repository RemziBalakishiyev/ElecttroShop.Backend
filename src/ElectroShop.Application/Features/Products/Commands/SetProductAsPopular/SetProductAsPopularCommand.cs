using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsPopular;

public record SetProductAsPopularCommand(Guid ProductId, int DisplayOrder) : IRequest<Result>;
