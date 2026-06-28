using FluentValidation;

namespace ElectroShop.Application.Features.ProductRatings.Commands.DeleteProductRating;

public class DeleteProductRatingCommandValidator : AbstractValidator<DeleteProductRatingCommand>
{
    public DeleteProductRatingCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si tələb olunur");
    }
}
