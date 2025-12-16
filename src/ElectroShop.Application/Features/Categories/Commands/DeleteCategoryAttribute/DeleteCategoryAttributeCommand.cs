using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttribute;

public record DeleteCategoryAttributeCommand(Guid Id) : IRequest<Result>;



