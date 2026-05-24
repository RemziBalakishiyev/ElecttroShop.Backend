using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsFeatured;

public class SetProductAsFeaturedCommandValidator : AbstractValidator<SetProductAsFeaturedCommand>
{
    public SetProductAsFeaturedCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Display order minimum 1 olmalıdır")
            .LessThanOrEqualTo(5)
            .WithMessage("Display order maksimum 5 ola bilər");
    }
}







