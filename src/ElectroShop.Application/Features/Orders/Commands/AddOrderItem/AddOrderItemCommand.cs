using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.AddOrderItem;

public record AddOrderItemCommand(
    Guid OrderId,
    Guid ProductId,
    int Quantity) : IRequest<Result<OrderDto>>;

