using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.SetProductAsBanner;

public class SetProductAsBannerCommandValidator : AbstractValidator<SetProductAsBannerCommand>
{
    public SetProductAsBannerCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }
}







