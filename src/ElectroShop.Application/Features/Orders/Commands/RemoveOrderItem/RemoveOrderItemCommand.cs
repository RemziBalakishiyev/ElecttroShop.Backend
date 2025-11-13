using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.RemoveOrderItem;

public record RemoveOrderItemCommand(
    Guid OrderId,
    Guid ProductId) : IRequest<Result<OrderDto>>;

