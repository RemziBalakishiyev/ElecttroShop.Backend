using ElectroShop.Domain.Enums;
using ElectroShop.Domain.Primitives;

namespace ElectroShop.Domain.Entities;

public class CreditSale : BaseCommonEntity
{
    public string? CustomerName { get; private set; }
    public string? CustomerPhone { get; private set; }
    public CreditSaleProductSource ProductSource { get; private set; }
    public Guid? ProductId { get; private set; }
    public Product? Product { get; private set; }
    public string ProductName { get; private set; } = default!;
    public string? ProductCode { get; private set; }
    public Guid? CategoryId { get; private set; }
    public string? CategoryName { get; private set; }
    public decimal CostPrice { get; private set; }
    public decimal SalePrice { get; private set; }
    public int Quantity { get; private set; }
    public decimal TotalCostAmount { get; private set; }
    public decimal TotalSaleAmount { get; private set; }
    public decimal TotalExpenses { get; private set; }
    public decimal GrossProfit { get; private set; }
    public decimal NetProfit { get; private set; }
    public DateTime CreditDate { get; private set; }
    public DateTime DueDate { get; private set; }
    public int DebtDurationDays { get; private set; }
    public CreditSaleStatus Status { get; private set; }
    public string? Note { get; private set; }
    public Guid? ConvertedSaleId { get; private set; }
    public Sale? ConvertedSale { get; private set; }
    public DateTime? ConvertedAt { get; private set; }

    public List<CreditSaleExpense> Expenses { get; private set; } = [];

    private CreditSale() { }

    public static CreditSale CreateManual(
        string? customerName,
        string? customerPhone,
        string productName,
        string? productCode,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime creditDate,
        DateTime dueDate,
        string? note = null)
    {
        ValidateProductName(productName);
        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateDates(creditDate, dueDate);

        var creditSale = new CreditSale
        {
            CustomerName = NormalizeOptional(customerName),
            CustomerPhone = NormalizeOptional(customerPhone),
            ProductSource = CreditSaleProductSource.Manual,
            ProductName = productName.Trim(),
            ProductCode = productCode?.Trim(),
            CostPrice = costPrice,
            SalePrice = salePrice,
            Quantity = quantity,
            CreditDate = creditDate,
            DueDate = dueDate,
            Status = CreditSaleStatus.Pending,
            Note = note?.Trim()
        };

        creditSale.RecalculateTotals();
        return creditSale;
    }

    public static CreditSale CreateFromSystemProduct(
        string? customerName,
        string? customerPhone,
        Guid productId,
        string productName,
        string? productCode,
        Guid? categoryId,
        string? categoryName,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime creditDate,
        DateTime dueDate,
        string? note = null)
    {
        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateDates(creditDate, dueDate);

        if (productId == Guid.Empty)
            throw new ArgumentException("Məhsul ID-si boş ola bilməz", nameof(productId));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Məhsul adı boş ola bilməz", nameof(productName));

        var creditSale = new CreditSale
        {
            CustomerName = NormalizeOptional(customerName),
            CustomerPhone = NormalizeOptional(customerPhone),
            ProductSource = CreditSaleProductSource.SystemProduct,
            ProductId = productId,
            ProductName = productName.Trim(),
            ProductCode = productCode?.Trim(),
            CategoryId = categoryId,
            CategoryName = categoryName?.Trim(),
            CostPrice = costPrice,
            SalePrice = salePrice,
            Quantity = quantity,
            CreditDate = creditDate,
            DueDate = dueDate,
            Status = CreditSaleStatus.Pending,
            Note = note?.Trim()
        };

        creditSale.RecalculateTotals();
        return creditSale;
    }

    public void UpdatePending(
        string? customerName,
        string? customerPhone,
        decimal costPrice,
        decimal salePrice,
        int quantity,
        DateTime creditDate,
        DateTime dueDate,
        string? note)
    {
        EnsureEditable();

        ValidatePrices(costPrice, salePrice);
        ValidateQuantity(quantity);
        ValidateDates(creditDate, dueDate);

        CustomerName = NormalizeOptional(customerName);
        CustomerPhone = NormalizeOptional(customerPhone);
        CostPrice = costPrice;
        SalePrice = salePrice;
        Quantity = quantity;
        CreditDate = creditDate;
        DueDate = dueDate;
        Note = note?.Trim();
        RecalculateTotals();
    }

    public void SetExpenses(IReadOnlyList<SaleExpenseDraft> expenseDrafts)
    {
        ArgumentNullException.ThrowIfNull(expenseDrafts);

        foreach (var draft in expenseDrafts)
        {
            var expense = CreditSaleExpense.Create(draft.ExpenseType, draft.Amount, draft.Description);
            expense.AttachToCreditSale(Id);
            Expenses.Add(expense);
        }

        RecalculateTotals();
    }

    public void ReplaceExpenses(IReadOnlyList<SaleExpenseDraft> expenseDrafts)
    {
        ArgumentNullException.ThrowIfNull(expenseDrafts);

        foreach (var expense in Expenses)
            expense.MarkDeleted();

        foreach (var draft in expenseDrafts)
        {
            var expense = CreditSaleExpense.Create(draft.ExpenseType, draft.Amount, draft.Description);
            expense.AttachToCreditSale(Id);
            Expenses.Add(expense);
        }

        RecalculateTotals();
    }

    public void MarkAsSold(Guid saleId, DateTime convertedAt)
    {
        if (Status != CreditSaleStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləyən nisyə satıla bilər");

        if (saleId == Guid.Empty)
            throw new ArgumentException("Satış ID-si boş ola bilməz", nameof(saleId));

        Status = CreditSaleStatus.Sold;
        ConvertedSaleId = saleId;
        ConvertedAt = convertedAt;
    }

    public void Cancel()
    {
        if (Status != CreditSaleStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləyən nisyə ləğv edilə bilər");

        Status = CreditSaleStatus.Cancelled;
    }

    public bool IsOverdue(DateTime todayUtc)
    {
        return Status == CreditSaleStatus.Pending && DueDate.Date < todayUtc.Date;
    }

    private void RecalculateTotals()
    {
        var totalExpenses = Expenses.Where(e => !e.IsDeleted).Sum(e => e.Amount);
        TotalCostAmount = CostPrice * Quantity;
        TotalSaleAmount = SalePrice * Quantity;
        TotalExpenses = totalExpenses;
        GrossProfit = TotalSaleAmount - TotalCostAmount;
        NetProfit = TotalSaleAmount - TotalCostAmount - totalExpenses;
        DebtDurationDays = (DueDate.Date - CreditDate.Date).Days;
    }

    private void EnsureEditable()
    {
        if (Status != CreditSaleStatus.Pending)
            throw new InvalidOperationException("Yalnız gözləyən və ya vaxtı keçmiş nisyə redaktə edilə bilər");
    }

    private static string? NormalizeOptional(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void ValidateProductName(string productName)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Məhsul adı boş ola bilməz", nameof(productName));
    }

    private static void ValidatePrices(decimal costPrice, decimal salePrice)
    {
        if (costPrice < 0)
            throw new ArgumentException("Maya dəyəri mənfi ola bilməz", nameof(costPrice));

        if (salePrice <= 0)
            throw new ArgumentException("Satış qiyməti 0-dan böyük olmalıdır", nameof(salePrice));
    }

    private static void ValidateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Miqdar 0-dan böyük olmalıdır", nameof(quantity));
    }

    private static void ValidateDates(DateTime creditDate, DateTime dueDate)
    {
        if (dueDate.Date < creditDate.Date)
            throw new ArgumentException("Son ödəniş tarixi nisyə tarixindən kiçik ola bilməz", nameof(dueDate));
    }
}
