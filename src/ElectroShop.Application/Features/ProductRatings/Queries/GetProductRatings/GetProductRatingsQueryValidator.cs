using FluentValidation;

namespace ElectroShop.Application.Features.ProductRatings.Queries.GetProductRatings;

public class GetProductRatingsQueryValidator : AbstractValidator<GetProductRatingsQuery>
{
    public GetProductRatingsQueryValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .WithMessage("Məhsul ID-si tələb olunur");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Səhifə nömrəsi 0-dan böyük olmalıdır");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100)
            .WithMessage("Səhifə ölçüsü 1 ilə 100 arasında olmalıdır");
    }
}
