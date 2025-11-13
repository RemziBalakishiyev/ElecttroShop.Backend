using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Events;

public sealed class ProductPriceChanged : IDomainEvent
{
    public Guid ProductId { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public ProductPriceChanged(Guid productId, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}
