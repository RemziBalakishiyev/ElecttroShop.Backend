using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetPrimaryImage;

public record SetPrimaryImageCommand(Guid ProductId, Guid ImageId) : IRequest<Result>;


