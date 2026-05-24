using System.Text.Json;
using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;

namespace ElectroShop.Application.Services;

public class ProductVariantAttributeValidator : IProductVariantAttributeValidator
{
    public Result<List<NormalizedProductVariant>> ValidateAndNormalize(
        ResolvedCategoryAttributeSchema schema,
        IReadOnlyList<ProductVariantRequestDto> variants,
        CategoryChangeContext? categoryChange)
    {
        if (categoryChange is not null
            && categoryChange.OldCategoryId != categoryChange.NewCategoryId
            && categoryChange.ExistingVariantAttributesJson.Count > 0
            && variants.Count == 0)
        {
            return Result.Failure<List<NormalizedProductVariant>>(
                DomainErrors.ProductVariant.CategoryChangeIncompatible);
        }

        if (categoryChange is not null
            && categoryChange.OldCategoryId != categoryChange.NewCategoryId)
        {
            var categoryChangeResult = ValidateCategoryChangeCompatibility(schema, variants, categoryChange);
            if (categoryChangeResult.IsFailure)
                return Result.Failure<List<NormalizedProductVariant>>(categoryChangeResult.Error);
        }

        var normalizedVariants = new List<NormalizedProductVariant>();
        var activeFingerprints = new HashSet<string>(StringComparer.Ordinal);

        foreach (var variant in variants)
        {
            if (variant.Attributes is null || variant.Attributes.Count == 0)
            {
                return Result.Failure<List<NormalizedProductVariant>>(
                    DomainErrors.ProductVariant.EmptyAttributes);
            }

            var canonicalMapResult = BuildCanonicalAttributeMap(schema, variant.Attributes);
            if (canonicalMapResult.IsFailure)
                return Result.Failure<List<NormalizedProductVariant>>(canonicalMapResult.Error);

            var canonicalMap = canonicalMapResult.Value;

            var requiredResult = ValidateRequiredAttributes(schema, canonicalMap);
            if (requiredResult.IsFailure)
                return Result.Failure<List<NormalizedProductVariant>>(requiredResult.Error);

            var attributesJson = VariantAttributeComparer.CreateFingerprint(canonicalMap);

            if (variant.IsActive && !activeFingerprints.Add(attributesJson))
            {
                return Result.Failure<List<NormalizedProductVariant>>(
                    DomainErrors.ProductVariant.DuplicateCombination);
            }

            normalizedVariants.Add(new NormalizedProductVariant(
                variant.Id,
                attributesJson,
                variant.ImageId,
                variant.IsActive));
        }

        return Result.Success(normalizedVariants);
    }

    private static Result ValidateCategoryChangeCompatibility(
        ResolvedCategoryAttributeSchema schema,
        IReadOnlyList<ProductVariantRequestDto> variants,
        CategoryChangeContext categoryChange)
    {
        foreach (var variant in variants.Where(v => v.Id.HasValue))
        {
            var variantId = variant.Id!.Value;
            if (!categoryChange.ExistingVariantAttributesJson.TryGetValue(variantId, out var oldJson))
                continue;

            if (string.IsNullOrWhiteSpace(oldJson))
                continue;

            Dictionary<string, string>? oldAttributes;
            try
            {
                oldAttributes = JsonSerializer.Deserialize<Dictionary<string, string>>(oldJson);
            }
            catch (JsonException)
            {
                return Result.Failure(DomainErrors.ProductVariant.CategoryChangeIncompatible);
            }

            if (oldAttributes is null || oldAttributes.Count == 0)
                continue;

            foreach (var oldKey in oldAttributes.Keys)
            {
                var resolvedInNewSchema = schema.FindByType(oldKey) is not null;
                var presentInRequest = variant.Attributes.Keys.Any(k =>
                    AttributeTypeNormalizer.Equals(k, oldKey));

                if (!resolvedInNewSchema && !presentInRequest)
                    return Result.Failure(DomainErrors.ProductVariant.CategoryChangeIncompatible);
            }
        }

        return Result.Success();
    }

    private static Result<Dictionary<string, string>> BuildCanonicalAttributeMap(
        ResolvedCategoryAttributeSchema schema,
        Dictionary<string, string> attributes)
    {
        var canonicalMap = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var kvp in attributes)
        {
            var attributeType = AttributeTypeNormalizer.Normalize(kvp.Key);
            if (string.IsNullOrEmpty(attributeType))
            {
                return Result.Failure<Dictionary<string, string>>(
                    DomainErrors.ProductVariant.AttributeNotFound(kvp.Key));
            }

            var attribute = schema.FindByType(attributeType);
            if (attribute is null)
            {
                return Result.Failure<Dictionary<string, string>>(
                    DomainErrors.ProductVariant.AttributeNotFound(attributeType));
            }

            var rawValue = AttributeTypeNormalizer.NormalizeValue(kvp.Value);
            if (string.IsNullOrEmpty(rawValue))
            {
                return Result.Failure<Dictionary<string, string>>(
                    DomainErrors.ProductVariant.EmptyAttributes);
            }

            var matchedValue = attribute.Values
                .FirstOrDefault(v => AttributeTypeNormalizer.ValueEquals(v.Value, rawValue));

            if (matchedValue is null)
            {
                return Result.Failure<Dictionary<string, string>>(
                    DomainErrors.ProductVariant.ValueNotFound(rawValue, attribute.AttributeType));
            }

            canonicalMap[attribute.AttributeType] = matchedValue.Value;
        }

        return Result.Success(canonicalMap);
    }

    private static Result ValidateRequiredAttributes(
        ResolvedCategoryAttributeSchema schema,
        IReadOnlyDictionary<string, string> canonicalMap)
    {
        foreach (var attribute in schema.AttributesByType.Values.Where(a => a.IsRequired))
        {
            if (!canonicalMap.ContainsKey(attribute.AttributeType))
            {
                return Result.Failure(
                    DomainErrors.ProductVariant.RequiredAttributeMissing(attribute.AttributeType));
            }
        }

        return Result.Success();
    }
}