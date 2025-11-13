using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryHandler : IRequestHandler<GetOrdersByCustomerQuery, PagedResult<OrderListDto>>
{
    private readonly IOrderQueryRepository _orderRepository;

    public GetOrdersByCustomerQueryHandler(IOrderQueryRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<PagedResult<OrderListDto>> Handle(
        GetOrdersByCustomerQuery request,
        CancellationToken cancellationToken)
    {
        var (orders, totalCount) = await _orderRepository.GetOrdersByCustomerPagedAsync(
            request.CustomerId,
            request.Page,
            request.PageSize,
            cancellationToken);

        if (totalCount == 0)
        {
            return PagedResult<OrderListDto>.Empty(request.Page, request.PageSize);
        }

        var orderDtos = orders.Adapt<List<OrderListDto>>();

        return PagedResult<OrderListDto>.Success(orderDtos, request.Page, request.PageSize, totalCount);
    }
}

