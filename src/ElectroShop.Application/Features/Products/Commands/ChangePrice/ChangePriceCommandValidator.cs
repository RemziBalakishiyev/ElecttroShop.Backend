using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.ChangePrice;

public class ChangePriceCommandValidator : AbstractValidator<ChangePriceCommand>
{
    public ChangePriceCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");

        RuleFor(x => x.NewPrice)
            .GreaterThan(0)
            .WithMessage("Yeni qiymət 0-dan böyük olmalıdır")
            .LessThanOrEqualTo(1_000_000)
            .WithMessage("Yeni qiymət 1,000,000-dan kiçik və ya bərabər olmalıdır");
    }
}

