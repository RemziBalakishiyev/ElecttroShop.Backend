namespace ElectroShop.Domain.Primitives;

public abstract class BaseCommonEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public bool IsDeleted { get; protected set; } = false;

    public void MarkDeleted(string? deletedBy = null)
    {
        IsDeleted = true;
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }

    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
}
