using FluentValidation;

namespace ElectroShop.Application.Features.Categories.Queries.GetCategoryBySlug;

public class GetCategoryBySlugQueryValidator : AbstractValidator<GetCategoryBySlugQuery>
{
    public GetCategoryBySlugQueryValidator()
    {
        RuleFor(x => x.Slug)
            .NotEmpty()
            .WithMessage("Slug boş ola bilməz")
            .Matches(@"^[a-z0-9\-]+$")
            .WithMessage("Yanlış slug formatı");
    }
}

