using ElectroShop.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElectroShop.Application.EventHandlers;

/// <summary>
/// ProductPriceChanged Domain Event Handler
/// Örnek: Fiyat değişikliğini loglayabilir, notification gönderebilir, vb.
/// </summary>
public class ProductPriceChangedHandler : INotificationHandler<ProductPriceChanged>
{
    private readonly ILogger<ProductPriceChangedHandler> _logger;

    public ProductPriceChangedHandler(ILogger<ProductPriceChangedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(ProductPriceChanged notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Product {ProductId} price changed from {OldPrice} to {NewPrice} at {OccurredOn}",
            notification.ProductId,
            notification.OldPrice,
            notification.NewPrice,
            notification.OccurredOnUtc);

        // Burada başka işlemler yapılabilir:
        // - Email gönderme
        // - Notification oluşturma
        // - Cache temizleme
        // - Analytics'e kaydetme
        // vb.

        return Task.CompletedTask;
    }
}



