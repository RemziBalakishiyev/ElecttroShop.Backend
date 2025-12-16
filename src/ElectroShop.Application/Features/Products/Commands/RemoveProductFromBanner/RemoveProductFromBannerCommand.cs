using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromBanner;

public record RemoveProductFromBannerCommand(Guid ProductId) : IRequest<Result>;




