using FluentValidation;

namespace ElectroShop.Application.Features.Products.Queries.GetProducts;

/// <summary>
/// GetProductsQuery üçün Validator
/// </summary>
public class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    public GetProductsQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0)
            .WithMessage("Səhifə nömrəsi 0-dan böyük olmalıdır");

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .WithMessage("Səhifə ölçüsü 0-dan böyük olmalıdır")
            .LessThanOrEqualTo(100)
            .WithMessage("Səhifə ölçüsü maksimum 100 ola bilər");

        When(x => x.MinPrice.HasValue, () =>
        {
            RuleFor(x => x.MinPrice!.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Minimum qiymət 0-dan kiçik ola bilməz");
        });

        When(x => x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x.MaxPrice!.Value)
                .GreaterThan(0)
                .WithMessage("Maksimum qiymət 0-dan böyük olmalıdır");
        });

        When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue, () =>
        {
            RuleFor(x => x)
                .Must(x => x.MinPrice!.Value <= x.MaxPrice!.Value)
                .WithMessage("Minimum qiymət maksimum qiymətdən böyük ola bilməz");
        });
    }
}

