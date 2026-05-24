using ElectroShop.Application.Abstractions;
using FluentValidation;

namespace ElectroShop.Application.Features.Shippers.Commands.CreateShipperByFF;

public class CreateShipperByFFCommandValidator : AbstractValidator<CreateShipperByFFCommand>
{
    private readonly IShipperQueryRepository _shipperRepository;
    private readonly IQueryRepository<Domain.Entities.ForwardingFreight> _forwardingFreightRepository;

    public CreateShipperByFFCommandValidator(
        IShipperQueryRepository shipperRepository,
        IQueryRepository<Domain.Entities.ForwardingFreight> forwardingFreightRepository)
    {
        _shipperRepository = shipperRepository;
        _forwardingFreightRepository = forwardingFreightRepository;

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

        RuleFor(x => x.ForwardingFreightId)
            .NotEmpty()
            .WithMessage("Forwarding Freight ID boş ola bilməz")
            .MustAsync(BeValidForwardingFreight)
            .WithMessage("Forwarding Freight tapılmadı və ya aktiv deyil");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Telefon nömrəsi maksimum 20 simvol ola bilər")
            .When(x => !string.IsNullOrWhiteSpace(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Ünvan maksimum 500 simvol ola bilər")
            .When(x => !string.IsNullOrWhiteSpace(x.Address));
    }

    private async Task<bool> BeUniqueEmail(string email, CancellationToken cancellationToken)
    {
        var existing = await _shipperRepository.GetShipperByEmailAsync(email, cancellationToken);
        return existing is null;
    }

    private async Task<bool> BeValidForwardingFreight(Guid forwardingFreightId, CancellationToken cancellationToken)
    {
        var ff = await _forwardingFreightRepository.GetByIdAsync(forwardingFreightId, cancellationToken);
        return ff != null && ff.IsActive;
    }
}

