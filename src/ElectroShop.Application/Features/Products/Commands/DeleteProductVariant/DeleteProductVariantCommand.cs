using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProductVariant;

public record DeleteProductVariantCommand(Guid ProductId, Guid VariantId) : IRequest<Result>;






