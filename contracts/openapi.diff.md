# OpenAPI Diff — Credit Sale Field Updates

## Date
2026-07-09

## Summary
Nisyə modulunda müştəri sahələri optional edildi, xərclər Sales modulu kimi çoxsətirli struktura keçirildi, manual girişdən kateqoriya çıxarıldı.

## Breaking Changes (Admin Frontend)

### CreateCreditSaleCommand / UpdateCreditSaleCommand
| Old | New |
|-----|-----|
| `customerName` required | `customerName` optional (nullable) |
| `customerPhone` required | `customerPhone` optional (nullable) |
| `expenses` (decimal) | **removed** |
| `categoryId` (manual) | **removed from request** |
| — | `expenses` (array of `SaleExpenseRequestDto`) |

### Response DTOs
| Old | New |
|-----|-----|
| `expenses` (decimal, list item) | `totalExpenses` (decimal) |
| detail: no expense list | detail: `expenses` (array of `SaleExpenseDto`) |
| `customerName` / `customerPhone` string | nullable string |

### SaleExpenseRequestDto (same as Sales)
```json
{
  "expenseType": "Installation",
  "description": "Quraşdırma",
  "amount": 25.00
}
```

ExpenseType values: `Installation`, `Delivery`, `Service`, `Commission`, `Other`

## Database
- Migration: `20260709175626_UpdateCreditSaleExpensesAndNullableCustomer`
- New table: `CreditSaleExpenses`
- `CreditSales.Expenses` renamed to `TotalExpenses`

## Non-breaking
- Endpoint URLs unchanged
- System product still returns `categoryId`/`categoryName` from product snapshot in response
