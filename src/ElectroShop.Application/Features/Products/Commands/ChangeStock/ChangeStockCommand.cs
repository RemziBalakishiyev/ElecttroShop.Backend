using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.ChangeStock;

public record ChangeStockCommand(
    Guid ProductId, 
    int Quantity, 
    StockOperation Operation) : IRequest<Result>;

public enum StockOperation
{
    Increase = 1,
    Decrease = 2
}

