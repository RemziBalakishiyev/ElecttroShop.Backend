using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.CreateOrder;

public record CreateOrderCommand(Guid CustomerId) : IRequest<Result<OrderDto>>;

