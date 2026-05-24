namespace ElectroShop.Application.DTOs;

public record InlineProductAttributeDto
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string AttributeType { get; init; } = string.Empty;
    public bool IsRequired { get; init; }
    public int DisplayOrder { get; init; }
    public List<InlineProductAttributeValueDto> Values { get; init; } = [];
}

public record InlineProductAttributeValueDto
{
    public string Value { get; init; } = string.Empty;
    public string? DisplayValue { get; init; }
    public int DisplayOrder { get; init; }
    public string? ColorCode { get; init; }
}

public record ProductVariantRequestDto
{
    public Guid? Id { get; init; }
    public Guid? ImageId { get; init; }
    public Dictionary<string, string> Attributes { get; init; } = new();
    public bool IsActive { get; init; } = true;
}