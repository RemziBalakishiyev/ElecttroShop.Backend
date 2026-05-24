using ElectroShop.Application.Abstractions;
using ElectroShop.Application.Common;
using ElectroShop.Application.Common.Results;
using ElectroShop.Application.DTOs;
using ElectroShop.Application.Models;
using ElectroShop.Domain.Entities;

namespace ElectroShop.Application.Services;

public partial class ProductAttributeSchemaResolver : IProductAttributeSchemaResolver
{
    private readonly ICategoryQueryRepository _categoryQueryRepository;
    private readonly IWriteRepository<CategoryAttribute> _attributeWriteRepository;
    private readonly IQueryRepository<Category> _categoryRepository;

    public ProductAttributeSchemaResolver(
        ICategoryQueryRepository categoryQueryRepository,
        IWriteRepository<CategoryAttribute> attributeWriteRepository,
        IQueryRepository<Category> categoryRepository)
    {
        _categoryQueryRepository = categoryQueryRepository;
        _attributeWriteRepository = attributeWriteRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<Result<ResolvedCategoryAttributeSchema>> ResolveAsync(
        Guid categoryId,
        IReadOnlyList<InlineProductAttributeDto>? inlineAttributes,
        IReadOnlyList<Dictionary<string, string>> variantAttributeMaps,
        CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            return DomainErrors.Category.NotFound(categoryId);

        var inlineList = inlineAttributes ?? [];
        var inlineDuplicate = FindDuplicateAttributeTypes(inlineList);
        if (inlineDuplicate is not null)
            return DomainErrors.ProductVariant.AttributeAlreadyExists(inlineDuplicate);

        foreach (var inline in inlineList)
        {
            var valueDuplicate = FindDuplicateValues(inline.Values.Select(v => v.Value));
            if (valueDuplicate is not null)
                return DomainErrors.ProductVariant.ValueAlreadyExists(valueDuplicate, inline.AttributeType);
        }

        var trackedAttributes = await _categoryQueryRepository
            .GetCategoryAttributesForUpdateAsync(categoryId, cancellationToken);

        var inlineByType = BuildInlineLookup(inlineList);
        var requiredTypes = CollectRequiredAttributeTypes(inlineList, variantAttributeMaps);
        var attributesByType = new Dictionary<string, CategoryAttribute>(StringComparer.OrdinalIgnoreCase);

        foreach (var existingAttribute in trackedAttributes)
            attributesByType[existingAttribute.AttributeType] = existingAttribute;

        foreach (var incomingType in requiredTypes)
        {
            if (attributesByType.Keys.Any(k => AttributeTypeNormalizer.Equals(k, incomingType)))
                continue;

            inlineByType.TryGetValue(NormalizeTypeKey(incomingType), out var inlineDef);
            var maxDisplayOrder = trackedAttributes.Count > 0 ? trackedAttributes.Max(a => a.DisplayOrder) : -1;
            var attributeTypeToStore = GetPreferredAttributeType(incomingType, inlineDef, variantAttributeMaps);
            var name = inlineDef?.Name ?? attributeTypeToStore;
            var displayName = inlineDef?.DisplayName ?? attributeTypeToStore;
            var isRequired = inlineDef?.IsRequired ?? false;
            var displayOrder = inlineDef?.DisplayOrder ?? maxDisplayOrder + 1;

            try
            {
                var created = CategoryAttribute.Create(
                    categoryId, name, displayName, attributeTypeToStore, isRequired, displayOrder);
                await _attributeWriteRepository.AddAsync(created, cancellationToken);
                trackedAttributes.Add(created);
                attributesByType[created.AttributeType] = created;
            }
            catch (ArgumentException ex)
            {
                return Result.Failure<ResolvedCategoryAttributeSchema>(
                    Error.Validation("ProductVariant.InvalidAttribute", ex.Message));
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
                    await _categoryQueryRepository.AddCategoryAttributeValueAsync(createdValue, cancellationToken);
                    attribute.Values.Add(createdValue);
                }
                catch (ArgumentException ex)
                {
                    return Result.Failure<ResolvedCategoryAttributeSchema>(
                        Error.Validation("ProductVariant.InvalidAttributeValue", ex.Message));
                }
            }
        }

        return Result.Success(new ResolvedCategoryAttributeSchema { AttributesByType = attributesByType });
    }
}