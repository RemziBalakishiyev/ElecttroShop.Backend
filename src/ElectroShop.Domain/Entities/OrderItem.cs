using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;

namespace ElectroShop.Domain.Entities;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public Product Product { get; private set; } = default!;
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; } = Money.Zero();
    public decimal VatRate { get; private set; }
    public Money LineTotal { get; private set; } = Money.Zero();

    private OrderItem() { }

    public OrderItem(Guid productId, int quantity, Money unitPrice, decimal vatRate)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
        VatRate = vatRate;
        CalculateLineTotal();
    }

    internal void SetOrderId(Guid orderId)
    {
        if (orderId == Guid.Empty)
            throw new ArgumentException("Sifariş ID-si boş ola bilməz", nameof(orderId));
        
        OrderId = orderId;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Miqdar 0-dan böyük olmalıdır", nameof(quantity));

        Quantity = quantity;
        CalculateLineTotal();
    }

    private void CalculateLineTotal()
    {
        LineTotal = new Money(Quantity * UnitPrice.Amount, UnitPrice.Currency);
    }
}
