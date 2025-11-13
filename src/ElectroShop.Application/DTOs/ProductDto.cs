namespace ElectroShop.Application.DTOs;

/// <summary>
/// Product Data Transfer Object
/// </summary>
public record ProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;
    public Guid BrandId { get; init; }
    public string BrandName { get; init; } = string.Empty;
    public decimal VatRate { get; init; }
    public int Stock { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Product List DTO (lighter version for list operations)
/// </summary>
public record ProductListDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Currency { get; init; } = string.Empty;
    public string Sku { get; init; } = string.Empty;
    public string CategoryName { get; init; } = string.Empty;
    public string BrandName { get; init; } = string.Empty;
    public int Stock { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// Create Product DTO
/// </summary>
public record CreateProductDto
{
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "AZN";
    public string Sku { get; init; } = string.Empty;
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
}

/// <summary>
/// Update Product DTO
/// </summary>
public record UpdateProductDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public string Currency { get; init; } = "AZN";
    public Guid CategoryId { get; init; }
    public Guid BrandId { get; init; }
    public decimal VatRate { get; init; } = 0.18m;
    public int Stock { get; init; }
}

