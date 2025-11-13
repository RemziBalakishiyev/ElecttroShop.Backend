using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.MarkOrderPaid;

public record MarkOrderPaidCommand(Guid OrderId) : IRequest<Result<OrderDto>>;

