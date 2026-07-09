# OpenAPI Diff — Monthly Sales Report API

## Date
2026-07-09

## Summary
Hesabatlar moduluna ay/il üzrə JSON satış hesabatı endpoint-i əlavə edildi. Admin panel dashboard-u üçün summary, günlük qrafik, top məhsullar, kateqoriya/satış növü breakdown, mənfəət/zərər analizi və son satışlar qaytarılır.

## New Endpoints

### GET /api/reports/sales/monthly
- **Summary:** Seçilmiş ay üzrə satış hesabatını JSON formatında qaytarır (dashboard üçün)
- **Auth:** JWT Bearer token (mövcud `/api/sales` və `/api/dashboard` ilə eyni — `[Authorize]`)
- **Query params:**
  - `year` (int, required) — 2000..2100
  - `month` (int, required) — 1..12
- **Success:** `200 OK` — `MonthlySalesReportDto`
- **Errors:**
  - `400` — etibarsız `year` və ya `month`
  - `401` — autentifikasiya tələb olunur

## New Schemas
- `MonthlySalesReportDto`
- `MonthlySalesReportSummaryDto`
- `DailySalesReportDto`
- `TopProductReportDto`
- `CategorySalesReportDto`
- `SaleTypeReportDto`
- `ProfitLossProductReportDto`
- `MonthlySalesReportItemDto`

## Data Filter
- Satış tarixi: `SoldAt` (UTC)
- Interval: ayın 1-ci günü 00:00:00 UTC ≤ `SoldAt` < növbəti ayın 1-ci günü 00:00:00 UTC
- Data yoxdursa `404` yox — sıfırlı summary, ayın bütün günləri üçün `dailySales` (0 dəyərlərlə), boş listlər

## Summary Formulas
- `GrossProfit = TotalSalesAmount - TotalCostAmount`
- `NetProfit = TotalSalesAmount - TotalCostAmount - TotalExpenses`
- `AverageSaleAmount = TotalSalesAmount / SalesCount` (SalesCount > 0)
- `ProfitMarginPercent = NetProfit / TotalSalesAmount * 100` (TotalSalesAmount > 0)

## Breaking Changes
None — yalnız yeni endpoint və schema-lar əlavə edilib. Mövcud export endpointləri dəyişməyib.
