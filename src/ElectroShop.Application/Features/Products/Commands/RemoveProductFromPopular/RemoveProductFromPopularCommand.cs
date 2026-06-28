using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromPopular;

public record RemoveProductFromPopularCommand(Guid ProductId) : IRequest<Result>;
