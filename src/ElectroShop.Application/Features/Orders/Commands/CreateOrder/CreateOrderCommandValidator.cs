using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    private readonly IQueryRepository<Customer> _customerRepository;

    public CreateOrderCommandValidator(IQueryRepository<Customer> customerRepository)
    {
        _customerRepository = customerRepository;

        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Müştəri ID-si boş ola bilməz")
            .MustAsync(CustomerExists)
            .WithMessage("Müştəri tapılmadı");
    }

    private async Task<bool> CustomerExists(Guid customerId, CancellationToken cancellationToken)
    {
        return await _customerRepository.AnyAsync(c => c.Id == customerId && !c.IsDeleted, cancellationToken);
    }
}

