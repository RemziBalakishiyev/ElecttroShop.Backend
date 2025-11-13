using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.SearchProducts;

public class SearchProductsQueryValidator : AbstractValidator<SearchProductsQuery>
{
    public SearchProductsQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Axtarış mətni boş ola bilməz")
            .MinimumLength(2)
            .WithMessage("Axtarış mətni minimum 2 simvol olmalıdır")
            .MaximumLength(100)
            .WithMessage("Axtarış mətni maksimum 100 simvol ola bilər");

        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Səhifə nömrəsi 0-dan böyük olmalıdır");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Səhifə ölçüsü 0-dan böyük olmalıdır")
            .LessThanOrEqualTo(100)
            .WithMessage("Səhifə ölçüsü maksimum 100 ola bilər");
    }
}

