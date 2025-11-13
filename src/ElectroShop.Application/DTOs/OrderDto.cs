using ElectroShop.Domain.Enums;

namespace ElectroShop.Application.DTOs;

/// <summary>
/// Order Data Transfer Object
/// </summary>
public record OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public decimal Subtotal { get; init; }
    public string SubtotalCurrency { get; init; } = string.Empty;
    public decimal Vat { get; init; }
    public string VatCurrency { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public string TotalCurrency { get; init; } = string.Empty;
    public List<OrderItemDto> Items { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Order Item Data Transfer Object
/// </summary>
public record OrderItemDto
{
    public Guid Id { get; init; }
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public string UnitPriceCurrency { get; init; } = string.Empty;
    public decimal VatRate { get; init; }
    public decimal LineTotal { get; init; }
    public string LineTotalCurrency { get; init; } = string.Empty;
}

/// <summary>
/// Order List DTO (lighter version for list operations)
/// </summary>
public record OrderListDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public OrderStatus Status { get; init; }
    public decimal Total { get; init; }
    public string TotalCurrency { get; init; } = string.Empty;
    public int ItemsCount { get; init; }
    public DateTime CreatedAt { get; init; }
}

