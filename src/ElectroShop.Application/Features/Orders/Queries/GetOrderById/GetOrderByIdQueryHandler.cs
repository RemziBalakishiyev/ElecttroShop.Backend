using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Queries.GetOrderById;

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, Result<OrderDto>>
{
    private readonly IOrderQueryRepository _orderRepository;

    public GetOrderByIdQueryHandler(IOrderQueryRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<Result<OrderDto>> Handle(
        GetOrderByIdQuery request,
        CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return DomainErrors.Order.NotFound(request.OrderId);
        }

        var orderDto = order.Adapt<OrderDto>();

        return Result.Success(orderDto);
    }
}

