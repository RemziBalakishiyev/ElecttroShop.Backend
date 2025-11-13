namespace ElectroShop.Application.DTOs;

/// <summary>
/// Customer Data Transfer Object
/// </summary>
public record CustomerDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

