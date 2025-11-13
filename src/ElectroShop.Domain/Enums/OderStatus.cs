namespace ElectroShop.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,      // Sipariş oluşturuldu, ödeme bekleniyor
    Paid = 1,         // Ödeme tamamlandı
    Processing = 2,   // Sipariş hazırlanıyor
    Shipped = 3,      // Kargoya verildi
    Delivered = 4,    // Teslim edildi
    Cancelled = 5,    // İptal edildi
    Refunded = 6      // İade edildi
}
