namespace ElectroShop.Application.DTOs;

/// <summary>
/// Lookup DTO - Key-Value formatında məlumat üçün
/// Cache management üçün istifadə olunur
/// </summary>
public record LookupDto
{
    public string Key { get; init; } = string.Empty; // Guid string formatında
    public string Value { get; init; } = string.Empty; // Display name
}

/// <summary>
/// Lookup Response - Cache metadata ilə
/// </summary>
public record LookupResponse
{
    public List<LookupDto> Items { get; init; } = [];
    public DateTime CachedAt { get; init; }
    public string CacheKey { get; init; } = string.Empty;
}

