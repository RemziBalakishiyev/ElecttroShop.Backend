using ElectroShop.Application.Common.Results;
using MediatR;

namespace ElectroShop.Application.Features.Products.Commands.DeleteProduct;

/// <summary>
/// Məhsulu silmək üçün Command (Soft Delete)
/// Result pattern ilə uğur mesajı qaytarır
/// </summary>
public record DeleteProductCommand(Guid Id) : IRequest<Result>;

