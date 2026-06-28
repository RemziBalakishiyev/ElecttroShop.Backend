namespace ElectroShop.Application.DTOs;

/// <summary>
/// Ana səhifə "Məşhur Məhsullar" bölməsi üçün DTO
/// </summary>
public record PopularProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? ShortDescription { get; init; }
    public string? ImageUrl { get; init; }
    public int? DisplayOrder { get; init; }
}
