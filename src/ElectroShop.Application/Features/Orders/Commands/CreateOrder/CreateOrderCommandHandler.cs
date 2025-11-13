using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Result<OrderDto>>
{
    private readonly IWriteRepository<Order> _orderRepository;
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IQueryRepository<Customer> _customerRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateOrderCommandHandler(
        IWriteRepository<Order> orderRepository,
        IOrderQueryRepository orderQueryRepository,
        IQueryRepository<Customer> customerRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderQueryRepository = orderQueryRepository;
        _customerRepository = customerRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(
        CreateOrderCommand request,
        CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        if (customer is null)
        {
            return DomainErrors.Customer.NotFound(request.CustomerId);
        }

        Order order;
        try
        {
            order = Order.Create(request.CustomerId);
        }
        catch (ArgumentException ex)
        {
            return Result.Failure<OrderDto>(Error.Validation("Order.InvalidData", ex.Message));
        }

        await _orderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var orderWithDetails = await _orderQueryRepository.GetOrderWithDetailsAsync(order.Id, cancellationToken);

        if (orderWithDetails is null)
        {
            return DomainErrors.Order.NotFound(order.Id);
        }

        var orderDto = orderWithDetails.Adapt<OrderDto>();

        return Result.Success(orderDto);
    }
}

