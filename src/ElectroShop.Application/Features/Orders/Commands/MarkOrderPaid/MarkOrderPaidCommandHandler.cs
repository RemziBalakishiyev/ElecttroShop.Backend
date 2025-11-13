using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.MarkOrderPaid;

public class MarkOrderPaidCommandHandler : IRequestHandler<MarkOrderPaidCommand, Result<OrderDto>>
{
    private readonly IWriteRepository<Domain.Entities.Order> _orderRepository;
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MarkOrderPaidCommandHandler(
        IWriteRepository<Domain.Entities.Order> orderRepository,
        IOrderQueryRepository orderQueryRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderQueryRepository = orderQueryRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(
        MarkOrderPaidCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderQueryRepository.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return DomainErrors.Order.NotFound(request.OrderId);
        }

        try
        {
            order.MarkPaid();
        }
        catch (InvalidOperationException ex)
        {
            return Result.Failure<OrderDto>(Error.Validation("Order.InvalidOperation", ex.Message));
        }

        _orderRepository.Update(order);
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

