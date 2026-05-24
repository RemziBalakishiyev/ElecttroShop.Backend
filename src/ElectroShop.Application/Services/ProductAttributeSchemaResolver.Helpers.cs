using ElectroShop.Application.DTOs;
using ElectroShop.Application.Common;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Services;

public partial class ProductAttributeSchemaResolver
{
    private static string NormalizeTypeKey(string attributeType) =>
        AttributeTypeNormalizer.Normalize(attributeType).ToUpperInvariant();

    private static string? FindDuplicateAttributeTypes(IReadOnlyList<InlineProductAttributeDto> inlineList)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var inline in inlineList)
        {
            var key = AttributeTypeNormalizer.Normalize(inline.AttributeType);
            if (string.IsNullOrEmpty(key))
                continue;
            if (!seen.Add(key))
                return key;
        }
        return null;
    }

    private static string? FindDuplicateValues(IEnumerable<string> values)
    {
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var value in values)
        {
            var normalized = AttributeTypeNormalizer.NormalizeValue(value);
            if (string.IsNullOrEmpty(normalized))
                continue;
            if (!seen.Add(normalized))
                return normalized;
        }
        return null;
    }

    private static Dictionary<string, InlineProductAttributeDto> BuildInlineLookup(
        IReadOnlyList<InlineProductAttributeDto> inlineList)
    {
        var lookup = new Dictionary<string, InlineProductAttributeDto>(StringComparer.OrdinalIgnoreCase);
        foreach (var inline in inlineList)
        {
            var key = NormalizeTypeKey(inline.AttributeType);
            if (!string.IsNullOrEmpty(key))
                lookup[key] = inline;
        }
        return lookup;
    }

    private static HashSet<string> CollectRequiredAttributeTypes(
        IReadOnlyList<InlineProductAttributeDto> inlineList,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps)
    {
        var types = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var inline in inlineList)
        {
            var normalized = AttributeTypeNormalizer.Normalize(inline.AttributeType);
            if (!string.IsNullOrEmpty(normalized))
                types.Add(normalized);
        }
        foreach (var map in variantAttributeMaps)
        {
            foreach (var key in map.Keys)
            {
                var normalized = AttributeTypeNormalizer.Normalize(key);
                if (!string.IsNullOrEmpty(normalized))
                    types.Add(normalized);
            }
        }
        return types;
    }

    private static CategoryAttribute? FindExistingAttribute(
        IEnumerable<CategoryAttribute> attributes,
        string incomingType)
    {
        return attributes.FirstOrDefault(a => AttributeTypeNormalizer.Equals(a.AttributeType, incomingType));
    }

    private static string GetPreferredAttributeType(
        string incomingType,
        InlineProductAttributeDto? inlineDef,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps)
    {
        if (inlineDef is not null && !string.IsNullOrWhiteSpace(inlineDef.AttributeType))
            return AttributeTypeNormalizer.Normalize(inlineDef.AttributeType);
        foreach (var map in variantAttributeMaps)
        {
            var key = map.Keys.FirstOrDefault(k => AttributeTypeNormalizer.Equals(k, incomingType));
            if (key is not null)
                return AttributeTypeNormalizer.Normalize(key);
        }
        return AttributeTypeNormalizer.Normalize(incomingType);
    }

    private static List<(string RawValue, InlineProductAttributeValueDto? InlineDef)> CollectValuesForAttribute(
        string canonicalType,
        IReadOnlyDictionary<string, InlineProductAttributeDto> inlineByType,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps)
    {
        var results = new List<(string, InlineProductAttributeValueDto?)>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        inlineByType.TryGetValue(NormalizeTypeKey(canonicalType), out var inlineDef);
        if (inlineDef is not null)
        {
            foreach (var inlineValue in inlineDef.Values)
            {
                var normalized = AttributeTypeNormalizer.NormalizeValue(inlineValue.Value);
                if (string.IsNullOrEmpty(normalized) || !seen.Add(normalized))
                    continue;
                results.Add((inlineValue.Value, inlineValue));
            }
        }
        foreach (var map in variantAttributeMaps)
        {
            foreach (var kvp in map)
            {
                if (!AttributeTypeNormalizer.Equals(kvp.Key, canonicalType))
                    continue;
                var normalized = AttributeTypeNormalizer.NormalizeValue(kvp.Value);
                if (string.IsNullOrEmpty(normalized) || !seen.Add(normalized))
                    continue;
                var inlineValueDef = inlineDef?.Values
                    .FirstOrDefault(v => AttributeTypeNormalizer.ValueEquals(v.Value, normalized));
                results.Add((kvp.Value, inlineValueDef));
            }
        }
        return results;
    }
}