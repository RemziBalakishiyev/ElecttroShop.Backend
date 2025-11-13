using ElectroShop.Application.Abstractions;
using ElectroShop.Domain.Entities;
using FluentValidation;

namespace ElectroShop.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    private readonly IQueryRepository<Customer> _customerRepository;
    private readonly ICustomerQueryRepository _customerQueryRepository;

    public UpdateCustomerCommandValidator(
        IQueryRepository<Customer> customerRepository,
        ICustomerQueryRepository customerQueryRepository)
    {
        _customerRepository = customerRepository;
        _customerQueryRepository = customerQueryRepository;

        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Müştəri ID-si boş ola bilməz")
            .MustAsync(CustomerExists)
            .WithMessage("Müştəri tapılmadı");

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
            .MustAsync((command, email, ct) => BeUniqueEmailOrSame(command, email, ct))
            .WithMessage("Bu e-poçt ünvanı artıq istifadə olunur")
            .When(x => !string.IsNullOrWhiteSpace(x.Email));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Telefon nömrəsi maksimum 20 simvol ola bilər")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));
    }

    private async Task<bool> CustomerExists(Guid id, CancellationToken cancellationToken)
    {
        return await _customerRepository.AnyAsync(c => c.Id == id && !c.IsDeleted, cancellationToken);
    }

    private async Task<bool> BeUniqueEmailOrSame(UpdateCustomerCommand request, string email, CancellationToken cancellationToken)
    {
        var existing = await _customerQueryRepository.GetCustomerByEmailAsync(email, cancellationToken);
        return existing is null || existing.Id == request.Id;
    }
}

