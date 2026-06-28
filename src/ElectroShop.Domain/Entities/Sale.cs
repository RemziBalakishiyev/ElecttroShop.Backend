using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class Sale : BaseCommonEntity
{
    public Guid? ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string? ProductCode { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public decimal CostPrice { get; private set; }
    public decimal SalePrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalCost { get; private set; }
    public decimal TotalSaleAmount { get; private set; }
    public decimal Profit { get; private set; }
    public SaleSource SaleSource { get; private set; }
    public DateTime SoldAt { get; private set; }
    public string? Note { get; private set; }

    private Sale() { }

    public static Sale CreateFromExistingProduct(
        Guid productId,
        string productName,
        string? productCode,
        Guid? categoryId,
        string? categoryName,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime soldAt,
        string? note = null)
    {
        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateSoldAt(soldAt);

        if (productId == Guid.Empty)
            throw new ArgumentException("Məhsul ID-si boş ola bilməz", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Məhsul adı boş ola bilməz", nameof(productName));

        var (totalCost, totalSaleAmount, profit) = CalculateTotals(costPrice, salePrice, quantity);

        return new Sale
        {
            ProductId = productId,
            ProductName = productName.Trim(),
            ProductCode = productCode?.Trim(),
            CategoryId = categoryId,
            CategoryName = categoryName?.Trim(),
            CostPrice = costPrice,
            SalePrice = salePrice,
            Quantity = quantity,
            TotalCost = totalCost,
            TotalSaleAmount = totalSaleAmount,
            Profit = profit,
            SaleSource = SaleSource.ExistingProduct,
            SoldAt = soldAt,
            Note = note?.Trim()
        };
    }

    public static Sale CreateManualEntry(
        string productName,
        string? productCode,
        Guid? categoryId,
        string? categoryName,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime soldAt,
        string? note = null)
    {
        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateSoldAt(soldAt);

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Məhsul adı boş ola bilməz", nameof(productName));

        var (totalCost, totalSaleAmount, profit) = CalculateTotals(costPrice, salePrice, quantity);

        return new Sale
        {
            ProductName = productName.Trim(),
            ProductCode = productCode?.Trim(),
            CategoryId = categoryId,
            CategoryName = categoryName?.Trim(),
            CostPrice = costPrice,
            SalePrice = salePrice,
            Quantity = quantity,
            TotalCost = totalCost,
            TotalSaleAmount = totalSaleAmount,
            Profit = profit,
            SaleSource = SaleSource.ManualEntry,
            SoldAt = soldAt,
            Note = note?.Trim()
        };
    }

    public void UpdateExistingProductSale(
        decimal salePrice,
        int quantity,
        DateTime soldAt,
        string? note)
    {
        if (SaleSource != SaleSource.ExistingProduct)
            throw new InvalidOperationException("Yalnız mövcud məhsul satışı yenilənə bilər");

        ValidatePrices(CostPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateSoldAt(soldAt);

        SalePrice = salePrice;
        Quantity = quantity;
        SoldAt = soldAt;
        Note = note?.Trim();
        RecalculateTotals();
    }

    public void UpdateManualEntry(
        string productName,
        string? productCode,
        Guid? categoryId,
        string? categoryName,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime soldAt,
        string? note)
    {
        if (SaleSource != SaleSource.ManualEntry)
            throw new InvalidOperationException("Yalnız manual satış yenilənə bilər");

        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateSoldAt(soldAt);

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Məhsul adı boş ola bilməz", nameof(productName));

        ProductName = productName.Trim();
        ProductCode = productCode?.Trim();
        CategoryId = categoryId;
        CategoryName = categoryName?.Trim();
        CostPrice = costPrice;
        SalePrice = salePrice;
        Quantity = quantity;
        SoldAt = soldAt;
        Note = note?.Trim();
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        var (totalCost, totalSaleAmount, profit) = CalculateTotals(CostPrice, SalePrice, Quantity);
        TotalCost = totalCost;
        TotalSaleAmount = totalSaleAmount;
        Profit = profit;
    }

    private static (decimal TotalCost, decimal TotalSaleAmount, decimal Profit) CalculateTotals(
        decimal costPrice,
        decimal salePrice,
        int quantity)
    {
        var totalCost = costPrice * quantity;
        var totalSaleAmount = salePrice * quantity;
        var profit = (salePrice - costPrice) * quantity;
        return (totalCost, totalSaleAmount, profit);
    }

    private static void ValidatePrices(decimal costPrice, decimal salePrice)
    {
        if (costPrice < 0)
            throw new ArgumentException("Maya dəyəri mənfi ola bilməz", nameof(costPrice));

        if (salePrice < 0)
            throw new ArgumentException("Satış qiyməti mənfi ola bilməz", nameof(salePrice));
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Miqdar 0-dan böyük olmalıdır", nameof(quantity));
    }

    private static void ValidateSoldAt(DateTime soldAt)
    {
        if (soldAt > DateTime.UtcNow.AddDays(1))
            throw new ArgumentException("Satış tarixi gələcək ola bilməz", nameof(soldAt));

        if (soldAt < new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            throw new ArgumentException("Satış tarixi çox köhnədir", nameof(soldAt));
    }
}
