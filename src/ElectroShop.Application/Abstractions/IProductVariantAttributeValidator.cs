using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;

namespace ElectroShop.Application.Abstractions;

public interface IProductVariantAttributeValidator
{
    Result<List<NormalizedProductVariant>> ValidateAndNormalize(
        ResolvedCategoryAttributeSchema schema,
        IReadOnlyList<ProductVariantRequestDto> variants,
        CategoryChangeContext? categoryChange);
}