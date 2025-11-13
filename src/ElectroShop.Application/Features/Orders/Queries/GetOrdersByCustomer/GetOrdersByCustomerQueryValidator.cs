using FluentValidation;

namespace ElectroShop.Application.Features.Orders.Queries.GetOrdersByCustomer;

public class GetOrdersByCustomerQueryValidator : AbstractValidator<GetOrdersByCustomerQuery>
{
    public GetOrdersByCustomerQueryValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty()
            .WithMessage("Müştəri ID-si boş ola bilməz");

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

