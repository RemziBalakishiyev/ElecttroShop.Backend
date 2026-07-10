# Backend Change Result

## Summary
Nisyə modulunda: müştəri adı/telefonu optional, xərclər Sales kimi çoxsətirli, manual girişdə SKU optional və kateqoriya request-dən çıxarıldı.

## Changed Endpoints
Endpoint URL-ləri dəyişməyib. Request/response modelləri yenilənib.

### POST /api/credit-sales
### PUT /api/credit-sales/{id}

**Request dəyişiklikləri:**

| Sahə | Əvvəl | İndi |
|------|-------|------|
| `customerName` | required | optional (`null` ola bilər) |
| `customerPhone` | required | optional (`null` ola bilər) |
| `expenses` | `number` (tək məbləğ) | **silindi** |
| `expenses` | — | `SaleExpenseRequestDto[]` (Sales ilə eyni) |
| `categoryId` | optional (manual) | **request-dən silindi** |

**Manual giriş üçün tələb olunanlar:**
- `productSourceType`: 1
- `productName`
- `costPrice`, `salePrice`, `quantity`
- `creditDate`, `dueDate`
- `sku` — optional
- `customerName`, `customerPhone` — optional

**Xərc array nümunəsi (Sales ilə eyni):**
```json
"expenses": [
  { "expenseType": "Delivery", "description": "Çatdırılma", "amount": 15 },
  { "expenseType": "Other", "description": "Quraşdırma", "amount": 30 }
]
```

`expenseType` enum: `Installation`, `Delivery`, `Service`, `Commission`, `Other`

### GET /api/credit-sales (list)
- `expenses` → **`totalExpenses`** (cəmi məbləğ)
- `customerName`, `customerPhone` nullable

### GET /api/credit-sales/{id} (detail)
- `totalExpenses` — cəmi
- `expenses` — `SaleExpenseDto[]` (id, expenseType, description, amount, createdAt)
- `categoryId`/`categoryName` — yalnız sistem məhsulundan gələn snapshot (response-da qala bilər)

## Changed Models / DTOs
- `CreateCreditSaleCommand`: `expenses` array, `categoryId` yoxdur
- `UpdateCreditSaleCommand`: `expenses` array (göndərilsə tam əvəz olunur — Sales pattern)
- `CreditSaleListItemDto`: `totalExpenses` (köhnə `expenses` decimal silindi)
- `CreditSaleDetailDto`: + `expenses: SaleExpenseDto[]`

## Database / Business Rule Changes
- Migration: `20260709175626_UpdateCreditSaleExpensesAndNullableCustomer`
- `CreditSaleExpenses` cədvəli (Sales `SaleExpenses` kimi)
- `CreditSales.Expenses` → `TotalExpenses`
- `CustomerName`, `CustomerPhone` nullable
- Mark-as-sold: bütün xərc sətirləri Sales-ə köçürülür

## Frontend Impact

### Admin frontend — MÜTLƏQ dəyişikliklər

1. **Create/Edit form — Müştəri**
   - `customerName` və `customerPhone` required validasiyasını sil
   - Boş buraxıla bilər

2. **Create/Edit form — Manual giriş**
   - Kateqoriya seçimini sil (dropdown/input lazım deyil)
   - `categoryId` API-yə göndərmə
   - SKU sahəsi optional qalsın (required deyil)

3. **Create/Edit form — Xərclər (Sales səhifəsindən kopyala)**
   - Tək `expenses` number input-u sil
   - Sales modulundakı kimi dinamik sətir cədvəli əlavə et:
     - expenseType (select/enum)
     - description (optional text)
     - amount (number ≥ 0)
   - "Xərc əlavə et" / sil düymələri
   - API-yə `expenses: [{ expenseType, description, amount }]` göndər
   - Update zamanı `expenses` göndərilsə tam siyahı əvəz olunur (Sales ilə eyni)

4. **TypeScript types**
   - `expenses: number` → sil
   - `totalExpenses: number` (list)
   - `expenses: SaleExpenseDto[]` (detail)
   - `customerName?: string | null`
   - `customerPhone?: string | null`
   - Create/Update request-dən `categoryId` sil

5. **List cədvəli**
   - `expenses` sütununu `totalExpenses` ilə əvəz et

6. **Detail səhifəsi**
   - Xərclər bölməsində `expenses` array göstər (tip, təsvir, məbləğ)
   - Cəmi: `totalExpenses`

### User frontend
No frontend change required.

## OpenAPI
- contracts/openapi.json updated: manual refresh lazımdır (swagger export)
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: success (Persistence/Application/Domain)
- Migration: applied (`UpdateCreditSaleExpensesAndNullableCustomer`)

## Manual test
```json
POST /api/credit-sales
{
  "productSourceType": 1,
  "productName": "Test məhsul",
  "costPrice": 100,
  "salePrice": 150,
  "quantity": 1,
  "creditDate": "2026-07-01T00:00:00Z",
  "dueDate": "2026-07-31T00:00:00Z",
  "expenses": [
    { "expenseType": "Delivery", "amount": 10 },
    { "expenseType": "Other", "description": "Test", "amount": 5 }
  ]
}
```
