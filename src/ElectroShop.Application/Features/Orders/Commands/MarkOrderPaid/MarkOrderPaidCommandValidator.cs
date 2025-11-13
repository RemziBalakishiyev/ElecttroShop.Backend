using ElectroShop.Application.Abstractions;
using FluentValidation;

namespace ElectroShop.Application.Features.Orders.Commands.MarkOrderPaid;

public class MarkOrderPaidCommandValidator : AbstractValidator<MarkOrderPaidCommand>
{
    private readonly IOrderQueryRepository _orderRepository;

    public MarkOrderPaidCommandValidator(IOrderQueryRepository orderRepository)
    {
        _orderRepository = orderRepository;

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Sifariş ID-si boş ola bilməz")
            .MustAsync(OrderExists)
            .WithMessage("Sifariş tapılmadı");
    }

    private async Task<bool> OrderExists(Guid orderId, CancellationToken cancellationToken)
    {
        return await _orderRepository.AnyAsync(o => o.Id == orderId, cancellationToken);
    }
}

