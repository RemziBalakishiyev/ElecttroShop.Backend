using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Categories.Commands.DeleteCategoryAttributeValue;

public record DeleteCategoryAttributeValueCommand(Guid Id) : IRequest<Result>;






