namespace ElectroShop.Domain.Entities;

/// <summary>
/// Application log entry persisted for audit and diagnostics.
/// </summary>
public class AppLogEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string Level { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Exception { get; set; }

    public string? SourceContext { get; set; }

    public string? EventType { get; set; }

    public string? CorrelationId { get; set; }

    public Guid? UserId { get; set; }

    public string? UserEmail { get; set; }

    public string? RequestPath { get; set; }

    public string? RequestMethod { get; set; }

    public string? QueryString { get; set; }

    public string? RequestBody { get; set; }

    public int? ResponseStatusCode { get; set; }

    public long? ElapsedMilliseconds { get; set; }

    public string? ClientIp { get; set; }

    public string? UserAgent { get; set; }

    public string? MachineName { get; set; }

    public string? PropertiesJson { get; set; }
}
