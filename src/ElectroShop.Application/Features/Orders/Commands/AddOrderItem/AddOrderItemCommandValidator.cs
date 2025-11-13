using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Orders.Commands.AddOrderItem;

public class AddOrderItemCommandValidator : AbstractValidator<AddOrderItemCommand>
{
    private readonly IOrderQueryRepository _orderRepository;
    private readonly IQueryRepository<Product> _productRepository;

    public AddOrderItemCommandValidator(
        IOrderQueryRepository orderRepository,
        IQueryRepository<Product> productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;

        RuleFor(x => x.OrderId)
            .NotEmpty()
            .WithMessage("Sifariş ID-si boş ola bilməz")
            .MustAsync(OrderExists)
            .WithMessage("Sifariş tapılmadı");

        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz")
            .MustAsync(ProductExists)
            .WithMessage("Məhsul tapılmadı");

        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Miqdar 0-dan böyük olmalıdır")
            .LessThanOrEqualTo(1000)
            .WithMessage("Miqdar maksimum 1000 ola bilər");
    }

    private async Task<bool> OrderExists(Guid orderId, CancellationToken cancellationToken)
    {
        return await _orderRepository.AnyAsync(o => o.Id == orderId, cancellationToken);
    }

    private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
    {
        return await _productRepository.AnyAsync(p => p.Id == productId && !p.IsDeleted, cancellationToken);
    }
}

