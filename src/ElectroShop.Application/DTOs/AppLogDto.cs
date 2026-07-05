namespace ElectroShop.Application.DTOs;

public record AppLogDto
{
    public Guid Id { get; init; }
    public DateTime TimestampUtc { get; init; }
    public string Level { get; init; } = null!;
    public string Message { get; init; } = null!;
    public string? Exception { get; init; }
    public string? SourceContext { get; init; }
    public string? EventType { get; init; }
    public string? CorrelationId { get; init; }
    public Guid? UserId { get; init; }
    public string? UserEmail { get; init; }
    public string? RequestPath { get; init; }
    public string? RequestMethod { get; init; }
    public string? QueryString { get; init; }
    public string? RequestBody { get; init; }
    public int? ResponseStatusCode { get; init; }
    public long? ElapsedMilliseconds { get; init; }
    public string? ClientIp { get; init; }
    public string? UserAgent { get; init; }
    public string? MachineName { get; init; }
    public string? PropertiesJson { get; init; }
}
