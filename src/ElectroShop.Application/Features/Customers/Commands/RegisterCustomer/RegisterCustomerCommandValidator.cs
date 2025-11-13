using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Customers.Commands.RegisterCustomer;

public class RegisterCustomerCommandValidator : AbstractValidator<RegisterCustomerCommand>
{
    private readonly ICustomerQueryRepository _customerRepository;

    public RegisterCustomerCommandValidator(ICustomerQueryRepository customerRepository)
    {
        _customerRepository = customerRepository;

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage("Tam ad boş ola bilməz")
            .MaximumLength(200)
            .WithMessage("Tam ad maksimum 200 simvol ola bilər")
            .MinimumLength(2)
            .WithMessage("Tam ad minimum 2 simvol olmalıdır");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-poçt ünvanı boş ola bilməz")
            .EmailAddress()
            .WithMessage("Yanlış e-poçt ünvanı formatı")
            .MaximumLength(200)
            .WithMessage("E-poçt ünvanı maksimum 200 simvol ola bilər")
            .MustAsync(BeUniqueEmail)
            .WithMessage("Bu e-poçt ünvanı artıq istifadə olunur");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Telefon nömrəsi maksimum 20 simvol ola bilər")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existing = await _customerRepository.GetCustomerByEmailAsync(email, cancellationToken);
        return existing is null;
    }
}

