using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Services;

/// <summary>
/// Variant validasiyası üçün atribut sxemini qurur.
/// ÖNƏMLİ: Bu resolver artıq CategoryAttribute/CategoryAttributeValue-a HEÇ NƏ YAZMIR.
/// Sxem yalnız yaddaşda (in-memory), request-dəki inline atributlar və variant map-lərindən qurulur.
/// Beləliklə məhsul saxlanması kateqoriyadakı digər məhsullara təsir etmir.
/// </summary>
public partial class ProductAttributeSchemaResolver : IProductAttributeSchemaResolver
{
    public Task<Result<ResolvedCategoryAttributeSchema>> ResolveAsync(
        Guid categoryId,
        IReadOnlyList<InlineProductAttributeDto>? inlineAttributes,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps,
        CancellationToken cancellationToken)
    {
        var inlineList = inlineAttributes ?? [];

        var inlineDuplicate = FindDuplicateAttributeTypes(inlineList);
        if (inlineDuplicate is not null)
            return Task.FromResult(Result.Failure<ResolvedCategoryAttributeSchema>(
                DomainErrors.ProductVariant.AttributeAlreadyExists(inlineDuplicate)));

        foreach (var inline in inlineList)
        {
            var valueDuplicate = FindDuplicateValues(inline.Values.Select(v => v.Value));
            if (valueDuplicate is not null)
                return Task.FromResult(Result.Failure<ResolvedCategoryAttributeSchema>(
                    DomainErrors.ProductVariant.ValueAlreadyExists(valueDuplicate, inline.AttributeType)));
        }

        var inlineByType = BuildInlineLookup(inlineList);
        var requiredTypes = CollectRequiredAttributeTypes(inlineList, variantAttributeMaps);
        var attributesByType = new Dictionary<string, CategoryAttribute>(StringComparer.OrdinalIgnoreCase);

        var displayOrderCursor = -1;
        foreach (var incomingType in requiredTypes)
        {
            inlineByType.TryGetValue(NormalizeTypeKey(incomingType), out var inlineDef);
            var attributeTypeToStore = GetPreferredAttributeType(incomingType, inlineDef, variantAttributeMaps);
            var name = inlineDef?.Name ?? attributeTypeToStore;
            var displayName = inlineDef?.DisplayName ?? attributeTypeToStore;
            var isRequired = inlineDef?.IsRequired ?? false;
            var displayOrder = inlineDef?.DisplayOrder ?? ++displayOrderCursor;

            try
            {
                // Yalnız yaddaşda transient obyekt - DB-yə yazılmır
                var attribute = CategoryAttribute.Create(
                    categoryId, name, displayName, attributeTypeToStore, isRequired, displayOrder);
                attributesByType[attribute.AttributeType] = attribute;
            }
            catch (ArgumentException ex)
            {
                return Task.FromResult(Result.Failure<ResolvedCategoryAttributeSchema>(
                    Error.Validation("ProductVariant.InvalidAttribute", ex.Message)));
            }
        }

        foreach (var attribute in attributesByType.Values)
        {
            var valuesToEnsure = CollectValuesForAttribute(attribute.AttributeType, inlineByType, variantAttributeMaps);
            foreach (var (rawValue, inlineValueDef) in valuesToEnsure)
            {
                var normalizedValue = AttributeTypeNormalizer.NormalizeValue(rawValue);
                if (string.IsNullOrEmpty(normalizedValue))
                    continue;

                if (attribute.Values.Any(v => AttributeTypeNormalizer.ValueEquals(v.Value, normalizedValue)))
                    continue;

                try
                {
                    var displayOrder = inlineValueDef?.DisplayOrder
                        ?? (attribute.Values.Count > 0 ? attribute.Values.Max(v => v.DisplayOrder) + 1 : 0);
                    var createdValue = CategoryAttributeValue.Create(
                        attribute.Id, normalizedValue, inlineValueDef?.DisplayValue, displayOrder, inlineValueDef?.ColorCode);
                    attribute.Values.Add(createdValue);
                }
                catch (ArgumentException ex)
                {
                    return Task.FromResult(Result.Failure<ResolvedCategoryAttributeSchema>(
                        Error.Validation("ProductVariant.InvalidAttributeValue", ex.Message)));
                }
            }
        }

        return Task.FromResult(Result.Success(
            new ResolvedCategoryAttributeSchema { AttributesByType = attributesByType }));
    }
}
