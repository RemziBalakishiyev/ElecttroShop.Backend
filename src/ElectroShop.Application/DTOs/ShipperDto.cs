namespace ElectroShop.Application.DTOs;

/// <summary>
/// Shipper Data Transfer Object
/// </summary>
public record ShipperDto
{
    public Guid Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string? Address { get; init; }
    public bool IsActive { get; init; }
    public Guid? ForwardingFreightId { get; init; }
    public string? ForwardingFreightCompanyName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

