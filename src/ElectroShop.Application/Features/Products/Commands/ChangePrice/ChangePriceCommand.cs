using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.ChangePrice;

public record ChangePriceCommand(Guid ProductId, decimal NewPrice) : IRequest<Result>;

