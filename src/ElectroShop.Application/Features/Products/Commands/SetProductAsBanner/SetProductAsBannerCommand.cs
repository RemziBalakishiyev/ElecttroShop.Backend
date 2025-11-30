using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsBanner;

public record SetProductAsBannerCommand(Guid ProductId) : IRequest<Result>;


