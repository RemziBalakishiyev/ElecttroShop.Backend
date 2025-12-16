using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromBanner;

public class RemoveProductFromBannerCommandValidator : AbstractValidator<RemoveProductFromBannerCommand>
{
    public RemoveProductFromBannerCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }
}




