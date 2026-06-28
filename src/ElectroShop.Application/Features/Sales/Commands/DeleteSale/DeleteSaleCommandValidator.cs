using FluentValidation;

namespace ElectroShop.Application.Features.Sales.Commands.DeleteSale;

public class DeleteSaleCommandValidator : AbstractValidator<DeleteSaleCommand>
{
    public DeleteSaleCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}
