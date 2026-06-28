using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Sales.Commands.DeleteSale;

public record DeleteSaleCommand(Guid Id) : IRequest<Result>;
