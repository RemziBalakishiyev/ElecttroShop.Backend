using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromFeatured;

public class RemoveProductFromFeaturedCommandValidator : AbstractValidator<RemoveProductFromFeaturedCommand>
{
    public RemoveProductFromFeaturedCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }
}


