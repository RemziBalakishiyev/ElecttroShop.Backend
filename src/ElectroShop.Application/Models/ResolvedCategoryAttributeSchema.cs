using ElectroShop.Application.Common;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Models;

public sealed class ResolvedCategoryAttributeSchema
{
    public required IReadOnlyDictionary<string, CategoryAttribute> AttributesByType { get; init; }

    public CategoryAttribute? FindByType(string attributeType)
    {
        var key = AttributeTypeNormalizer.GetCanonicalKey(AttributesByType.Keys, attributeType);
        return AttributesByType.TryGetValue(key, out var attribute) ? attribute : null;
    }
}

public sealed record NormalizedProductVariant(
    Guid? Id,
    string AttributesJson,
    Guid? ImageId,
    bool IsActive);

public sealed record CategoryChangeContext(
    Guid OldCategoryId,
    Guid NewCategoryId,
    IReadOnlyDictionary<Guid, string> ExistingVariantAttributesJson);