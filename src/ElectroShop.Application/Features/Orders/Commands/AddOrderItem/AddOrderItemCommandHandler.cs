using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.ValueObjects;
using Mapster;
using MediatR;

namespace ElectroShop.Application.Features.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandHandler : IRequestHandler<AddOrderItemCommand, Result<OrderDto>>
{
    private readonly IWriteRepository<Order> _orderRepository;
    private readonly IOrderQueryRepository _orderQueryRepository;
    private readonly IQueryRepository<Product> _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddOrderItemCommandHandler(
        IWriteRepository<Order> orderRepository,
        IOrderQueryRepository orderQueryRepository,
        IQueryRepository<Product> productRepository,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _orderQueryRepository = orderQueryRepository;
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<OrderDto>> Handle(
        AddOrderItemCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _orderQueryRepository.GetOrderWithDetailsAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            return DomainErrors.Order.NotFound(request.OrderId);
        }

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            return DomainErrors.Product.NotFound(request.ProductId);
        }

        if (!product.IsActive)
        {
            return Result.Failure<OrderDto>(
                Error.Validation("Product.Inactive", "Məhsul aktiv deyil"));
        }

        if (product.Stock < request.Quantity)
        {
            return DomainErrors.Product.OutOfStock;
        }

        try
        {
            var orderItem = new OrderItem(
                request.ProductId,
                request.Quantity,
                product.Price,
                product.VatRate);

            order.AddItem(orderItem);
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

