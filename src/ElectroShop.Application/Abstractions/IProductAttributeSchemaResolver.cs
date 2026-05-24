using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;

namespace ElectroShop.Application.Abstractions;

public interface IProductAttributeSchemaResolver
{
    Task<Result<ResolvedCategoryAttributeSchema>> ResolveAsync(
        Guid categoryId,
        IReadOnlyList<InlineProductAttributeDto>? inlineAttributes,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps,
        CancellationToken cancellationToken);
}