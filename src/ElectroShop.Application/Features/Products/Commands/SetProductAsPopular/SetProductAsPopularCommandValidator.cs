using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsPopular;

public class SetProductAsPopularCommandValidator : AbstractValidator<SetProductAsPopularCommand>
{
    public SetProductAsPopularCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Display order minimum 1 olmalıdır")
            .LessThanOrEqualTo(4)
            .WithMessage("Display order maksimum 4 ola bilər");
    }
}
