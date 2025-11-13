using ElectroShop.Application.Abstractions;
using FluentValidation;

namespace ElectroShop.Application.Features.Orders.Commands.RemoveOrderItem;

public class RemoveOrderItemCommandValidator : AbstractValidator<RemoveOrderItemCommand>
{
    private readonly IOrderQueryRepository _orderRepository;

    public RemoveOrderItemCommandValidator(IOrderQueryRepository orderRepository)
    {
        _orderRepository = orderRepository;

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Sifariş ID-si boş ola bilməz")
            .MustAsync(OrderExists)
            .WithMessage("Sifariş tapılmadı");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }

    private async Task<bool> OrderExists(Guid orderId, CancellationToken cancellationToken)
    {
        return await _orderRepository.AnyAsync(o => o.Id == orderId, cancellationToken);
    }
}

