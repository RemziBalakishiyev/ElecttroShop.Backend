# Backend Change Result

## Summary
Hesabatlar moduluna ay və il üzrə JSON satış hesabatı API əlavə edildi. Admin paneldə ayrıca "Hesabatlar" səhifəsi bu endpoint-dən summary, günlük qrafik datası, top məhsullar, kateqoriya/satış növü breakdown, mənfəət/zərər analizi və son satışları göstərə bilər. Export endpointləri dəyişməyib.

## Changed Endpoints

### GET /api/reports/sales/monthly
- **Method:** GET
- **URL:** `/api/reports/sales/monthly?year={year}&month={month}`
- **Auth:** JWT Bearer token (`[Authorize]` — mövcud satışlar və dashboard ilə eyni)
- **Old behavior:** Endpoint mövcud deyildi
- **New behavior:** Seçilmiş təqvim ayı üzrə tam satış hesabatını JSON qaytarır
- **Request body:** yoxdur
- **Query params:**
  - `year` (required, int) — 2000..2100
  - `month` (required, int) — 1..12
- **Response body:** `MonthlySalesReportDto`
  - `year`, `month`, `monthName` (məs. "İYUL")
  - `startDate`, `endDate`, `generatedAt`
  - `summary` — `MonthlySalesReportSummaryDto`
  - `dailySales` — ayın bütün günləri (satış olmayan günlər 0 ilə)
  - `topProducts` — top 10 məhsul (quantity DESC)
  - `categorySales` — kateqoriya üzrə group
  - `saleTypeBreakdown` — satış növü üzrə group
  - `profitLossProducts` — ən yüksək və ən aşağı netProfit məhsullar (~10)
  - `recentSales` — son 20 satış (SoldAt DESC)
- **Validation rules:**
  - `year` — 2000-2100 arası
  - `month` — 1-12 arası
- **Error responses:**
  - `400` — etibarsız parametr
  - `401` — token yoxdur və ya etibarsızdır
- **Boş ay:** `200 OK`, summary sıfırlar, `dailySales` ayın bütün günləri 0 ilə

## Changed Models / DTOs
Yeni/ genişləndirilmiş JSON API modelləri:
- `MonthlySalesReportDto` — dashboard response (genişləndirildi)
- `MonthlySalesReportSummaryDto` — `averageSaleAmount`, `profitMarginPercent` əlavə
- `DailySalesReportDto`
- `TopProductReportDto`
- `CategorySalesReportDto`
- `SaleTypeReportDto`
- `ProfitLossProductReportDto`
- `MonthlySalesReportItemDto` — `grossProfit`, `netProfit` əlavə

Export üçün `Items` sahəsi saxlanılıb; JSON API response-da `items` boş array qaytarılır.

## Database / Business Rule Changes
- Migration yoxdur
- Satış filteri: `SoldAt >= ayın1ciGünüUTC && SoldAt < növbətiAyın1ciGünüUTC`
- Summary hesablamaları mövcud `GetSalesStatisticsAsync` aggregation ilə eynidir
- Kateqoriyasız məhsullar: `CategoryName = "Kateqoriyasız"`
- Sale type label: `SaleSourceDisplayHelper` ("Mövcud məhsul", "Manual giriş")

## Frontend Impact

### Admin
**Tələb olunan dəyişikliklər:**

1. **Yeni "Hesabatlar" səhifəsi yaradın** (sidebar/menu)
2. **Ay və il seçici** — `year`, `month` query paramları göndərilməlidir
3. **API çağırışı:**
   ```
   GET /api/reports/sales/monthly?year=2026&month=7
   Authorization: Bearer {token}
   ```
4. **Response-dan istifadə:**
   - Summary kartları: `summary.salesCount`, `summary.totalSalesAmount`, `summary.netProfit`, `summary.profitMarginPercent` və s.
   - Qrafik: `dailySales` array (bütün günlər mövcuddur)
   - Top məhsullar: `topProducts`
   - Kateqoriya chart: `categorySales`
   - Satış növü pie: `saleTypeBreakdown`
   - Mənfəət/zərər: `profitLossProducts`
   - Son satışlar cədvəli: `recentSales` (max 20)
5. **Boş ay:** UI error göstərməsin — sıfırlı data ilə render etsin

### User
No frontend change required.

## OpenAPI
- contracts/openapi.json updated: yes
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: Application və Persistence uğurlu; WebApi IIS Express file lock səbəbindən copy fail ola bilər (kod compile olunur)
- Backend tests: unit test layihəsi yoxdur
- Manual API test:
  1. JWT token ilə `GET /api/reports/sales/monthly?year=2026&month=7`
  2. Data olan ayda summary rəqəmlərini `GET /api/sales` eyni ay filteri ilə müqayisə et
  3. Boş ayda `dailySales.length` = ayın gün sayı, summary=0
  4. `month=13` → 400
  5. Auth olmadan → 401
- Known issues: WebApi build zamanı IIS Express/Visual Studio DLL lock ola bilər — serveri dayandırıb yenidən build edin
