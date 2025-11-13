using FluentValidation;

namespace ElectroShop.Application.Features.Brands.Queries.GetBrands;

public class GetBrandsQueryValidator : AbstractValidator<GetBrandsQuery>
{
    public GetBrandsQueryValidator()
    {
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

