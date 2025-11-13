using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategory;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;

