using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;
using ElectroShop.Domain.ValueObjects;

namespace ElectroShop.Domain.Entities;

public class Order : AggregateRoot
{
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; } = default!;
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public Money Subtotal { get; private set; } = Money.Zero();
    public Money Vat { get; private set; } = Money.Zero();
    public Money Total { get; private set; } = Money.Zero();

    public List<OrderItem> Items { get; private set; } = [];

    private Order() { }

    private Order(Guid customerId)
    {
        CustomerId = customerId;
        Status = OrderStatus.Pending;
        CalculateTotals();
    }

    public static Order Create(Guid customerId)
    {
        if (customerId == Guid.Empty)
            throw new ArgumentException("Müştəri ID-si boş ola bilməz", nameof(customerId));

        return new Order(customerId);
    }

    public void AddItem(OrderItem item)
    {
        if (item == null)
            throw new ArgumentNullException(nameof(item), "Sifariş elementi boş ola bilməz");

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləmədə olan sifarişlərdə element əlavə etmək olar");

        var existingItem = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existingItem != null)
        {
            existingItem.UpdateQuantity(existingItem.Quantity + item.Quantity);
        }
        else
        {
            var newItem = new OrderItem(item.ProductId, item.Quantity, item.UnitPrice, item.VatRate);
            newItem.SetOrderId(Id);
            Items.Add(newItem);
        }

        CalculateTotals();
    }

    public void RemoveItem(Guid productId)
    {
        if (productId == Guid.Empty)
            throw new ArgumentException("Məhsul ID-si boş ola bilməz", nameof(productId));

        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləmədə olan sifarişlərdə element silmək olar");

        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item == null)
            throw new InvalidOperationException("Sifariş elementində bu məhsul tapılmadı");

        Items.Remove(item);
        CalculateTotals();
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləmədə olan sifarişlər ödənilmiş kimi qeyd edilə bilər");

        if (Items.Count == 0)
            throw new InvalidOperationException("Boş sifariş ödənilmiş kimi qeyd edilə bilməz");

        Status = OrderStatus.Paid;
    }

    private void CalculateTotals()
    {
        if (Items.Count == 0)
        {
            Subtotal = Money.Zero();
            Vat = Money.Zero();
            Total = Money.Zero();
            return;
        }

        var subtotal = Items.Sum(i => i.LineTotal.Amount);
        var vat = Items.Sum(i => i.LineTotal.Amount * i.VatRate);
        Subtotal = new Money(subtotal, "AZN");
        Vat = new Money(vat, "AZN");
        Total = new Money(subtotal + vat, "AZN");
    }
}
