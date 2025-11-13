namespace ElectroShop.Application.DTOs;

/// <summary>
/// Category Data Transfer Object
/// </summary>
public record CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
    public Guid? ParentId { get; init; }
    public string? ParentName { get; init; }
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Category List DTO
/// </summary>
public record CategoryListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Slug { get; init; }
}

