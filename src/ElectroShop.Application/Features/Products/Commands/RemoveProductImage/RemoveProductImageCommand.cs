using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductImage;

public record RemoveProductImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result>;


