using ElectroShop.Application.DTOs;
using ElectroShop.Domain.Entities;
using Mapster;

namespace ElectroShop.Application.Mappings;

/// <summary>
/// Mapster configuration for Order entity mappings
/// </summary>
public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Order -> OrderDto
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.CustomerName, src => src.Customer != null ? src.Customer.FullName : string.Empty)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Subtotal, src => src.Subtotal.Amount)
            .Map(dest => dest.SubtotalCurrency, src => src.Subtotal.Currency)
            .Map(dest => dest.Vat, src => src.Vat.Amount)
            .Map(dest => dest.VatCurrency, src => src.Vat.Currency)
            .Map(dest => dest.Total, src => src.Total.Amount)
            .Map(dest => dest.TotalCurrency, src => src.Total.Currency)
            .Map(dest => dest.Items, src => src.Items)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc)
            .Map(dest => dest.UpdatedAt, src => src.UpdatedAtUtc);

        // OrderItem -> OrderItemDto
        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.ProductId, src => src.ProductId)
            .Map(dest => dest.ProductName, src => src.Product != null ? src.Product.Name : string.Empty)
            .Map(dest => dest.Quantity, src => src.Quantity)
            .Map(dest => dest.UnitPrice, src => src.UnitPrice.Amount)
            .Map(dest => dest.UnitPriceCurrency, src => src.UnitPrice.Currency)
            .Map(dest => dest.VatRate, src => src.VatRate)
            .Map(dest => dest.LineTotal, src => src.LineTotal.Amount)
            .Map(dest => dest.LineTotalCurrency, src => src.LineTotal.Currency);

        // Order -> OrderListDto
        config.NewConfig<Order, OrderListDto>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.CustomerId, src => src.CustomerId)
            .Map(dest => dest.CustomerName, src => src.Customer != null ? src.Customer.FullName : string.Empty)
            .Map(dest => dest.Status, src => src.Status)
            .Map(dest => dest.Total, src => src.Total.Amount)
            .Map(dest => dest.TotalCurrency, src => src.Total.Currency)
            .Map(dest => dest.ItemsCount, src => src.Items.Count)
            .Map(dest => dest.CreatedAt, src => src.CreatedAtUtc);
    }
}

