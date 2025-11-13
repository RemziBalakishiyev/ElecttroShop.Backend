using FluentValidation;

namespace ElectroShop.Application.Features.Auth.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-poçt ünvanı boş ola bilməz")
            .EmailAddress()
            .WithMessage("Yanlış e-poçt ünvanı formatı")
            .MaximumLength(200)
            .WithMessage("E-poçt ünvanı maksimum 200 simvol ola bilər");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Şifrə boş ola bilməz")
            .MinimumLength(6)
            .WithMessage("Şifrə minimum 6 simvol olmalıdır");
    }
}

