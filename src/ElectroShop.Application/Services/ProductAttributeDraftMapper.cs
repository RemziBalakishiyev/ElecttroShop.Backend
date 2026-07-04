using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Services;

/// <summary>
/// Request-dəki inline atributları məhsul səviyyəli domain draft-larına çevirir.
/// Boş/etibarsız atributlar və dəyərlər atlanır ki, domain istisnaları yaranmasın.
/// </summary>
public static class ProductAttributeDraftMapper
{
    public static List<ProductAttributeDraft> ToDrafts(IReadOnlyList<InlineProductAttributeDto>? inlineAttributes)
    {
        var drafts = new List<ProductAttributeDraft>();
        if (inlineAttributes is null || inlineAttributes.Count == 0)
            return drafts;

        var seenTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var inline in inlineAttributes)
        {
            var attributeType = inline.AttributeType?.Trim();
            if (string.IsNullOrWhiteSpace(attributeType))
                continue;

            if (!seenTypes.Add(attributeType))
                continue;

            var name = string.IsNullOrWhiteSpace(inline.Name) ? attributeType : inline.Name.Trim();
            var displayName = string.IsNullOrWhiteSpace(inline.DisplayName) ? name : inline.DisplayName.Trim();

            var values = new List<ProductAttributeValueDraft>();
            var seenValues = new HashSet<string>(StringComparer.Ordinal);

            foreach (var value in inline.Values ?? [])
            {
                var raw = value.Value?.Trim();
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (!seenValues.Add(raw))
                    continue;

                values.Add(new ProductAttributeValueDraft(
                    raw,
                    string.IsNullOrWhiteSpace(value.DisplayValue) ? null : value.DisplayValue.Trim(),
                    value.DisplayOrder,
                    string.IsNullOrWhiteSpace(value.ColorCode) ? null : value.ColorCode.Trim()));
            }

            drafts.Add(new ProductAttributeDraft(
                name,
                displayName,
                attributeType,
                inline.IsRequired,
                inline.DisplayOrder,
                values));
        }

        return drafts;
    }
}
