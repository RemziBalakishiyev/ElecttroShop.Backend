using FluentValidation;

namespace ElectroShop.Application.Features.Products.Commands.RemoveProductFromPopular;

public class RemoveProductFromPopularCommandValidator : AbstractValidator<RemoveProductFromPopularCommand>
{
    public RemoveProductFromPopularCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si boş ola bilməz");
    }
}
