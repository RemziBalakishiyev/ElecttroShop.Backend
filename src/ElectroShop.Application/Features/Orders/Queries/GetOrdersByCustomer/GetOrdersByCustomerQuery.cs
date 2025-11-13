using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Queries.GetOrdersByCustomer;

public record GetOrdersByCustomerQuery(
    Guid CustomerId,
    int Page = 1,
    int PageSize = 10) : IRequest<PagedResult<OrderListDto>>;

