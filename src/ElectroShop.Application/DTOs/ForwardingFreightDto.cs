namespace ElectroShop.Application.DTOs;

/// <summary>
/// Forwarding Freight Data Transfer Object
/// </summary>
public record ForwardingFreightDto
{
    public Guid Id { get; init; }
    public string CompanyName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public string? TaxId { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

