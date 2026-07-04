# Backend Change Result

## Summary

Admin Dashboard üçün yeni satış və məhsul statistik API-ləri əlavə olundu. Tək endpoint (`GET /api/dashboard/statistics`) günlük satış, aylıq satış və sistemdəki məhsul summary statistikalarını qaytarır. Köhnə `GET /api/dashboard` endpoint-i saxlanılıb (son məhsullar, son sifarişlər, order-based statistikalar); Admin Statistika səhifəsi yeni endpoint-ə keçməlidir.

**Timezone qeydi:** Bütün tarix filtrləri **UTC** əsasında hesablanır (`SoldAt` UTC saxlanılır; layihədə `DateTime.UtcNow` pattern-i istifadə olunur).

**Limitation:** `Product` entity-də ayrıca alış/original qiymət (`costPrice`) field-i yoxdur. Məhsul summary-də `totalProductCostValue` və `totalInventoryCostValue` müvəqqəti olaraq `Price.Amount` (satış qiyməti) əsasında hesablanır.

## Changed Endpoints

### GET /api/dashboard/statistics (YENİ)
- **Method:** GET
- **URL:** `/api/dashboard/statistics`
- **Auth:** `[Authorize]` — JWT tələb olunur (SalesController ilə eyni pattern)
- **Purpose:** Admin Dashboard statistikaları — günlük/aylıq satış və məhsul summary
- **Old behavior:** Endpoint mövcud deyildi
- **New behavior:** Günlük, aylıq satış və məhsul statistikalarını bir response-da qaytarır
- **Query params:** Yoxdur
- **Request body:** Yoxdur
- **Response body:**
  ```json
  {
    "dailySales": {
      "totalSaleAmount": 0,
      "totalProductCost": 0,
      "totalExpenses": 0,
      "totalProfit": 0,
      "soldProductQuantity": 0,
      "salesCount": 0
    },
    "monthlySales": {
      "totalSaleAmount": 0,
      "totalProductCost": 0,
      "totalExpenses": 0,
      "totalProfit": 0,
      "soldProductQuantity": 0,
      "salesCount": 0
    },
    "productSummary": {
      "totalProductCount": 0,
      "totalProductCostValue": 0,
      "totalProductSaleValue": 0,
      "totalInventoryCostValue": 0,
      "totalInventorySaleValue": 0
    }
  }
  ```
- **Validation/filtering rules:**
  - Satış statistikaları `SoldAt` field-i üzrə filtr olunur
  - Günlük: cari UTC günü (00:00 UTC – 24:00 UTC)
  - Aylıq: cari ayın 1-i (UTC) – bu günün sonu (UTC)
  - Soft deleted satış və məhsullar hesablamaya daxil edilmir
- **Error responses:** 401 Unauthorized

### GET /api/dashboard (DƏYİŞİKLİK — yalnız auth)
- **Method:** GET
- **URL:** `/api/dashboard`
- **Auth:** `[Authorize]` aktiv edildi (əvvəl comment olunmuşdu)
- **Purpose:** Köhnə dashboard (order statistikaları, son məhsullar/sifarişlər)
- **Old behavior:** Auth comment olunmuşdu (public)
- **New behavior:** JWT tələb olunur; response shape dəyişməyib
- **Query params:** Yoxdur
- **Request body:** Yoxdur
- **Response body:** `DashboardDto` (dəyişməyib)
- **Validation/filtering rules:** Dəyişməyib
- **Error responses:** 401 Unauthorized

### GET /api/dashboard/chart (DƏYİŞİKLİK — yalnız auth)
- **Method:** GET
- **URL:** `/api/dashboard/chart`
- **Auth:** `[Authorize]` (controller səviyyəsində aktiv)
- **Purpose:** Chart məlumatları
- **Old behavior:** Auth comment olunmuşdu
- **New behavior:** JWT tələb olunur; response dəyişməyib
- **Query params:** `period`, `periodCount`
- **Request body:** Yoxdur
- **Response body:** `ChartDataDto` (dəyişməyib)
- **Error responses:** 401 Unauthorized

## Changed Models / DTOs

### Yeni DTO-lar
- `DashboardStatisticsResponse` — `dailySales`, `monthlySales`, `productSummary`
- `SalesStatisticsResponse` — `totalSaleAmount`, `totalProductCost`, `totalExpenses`, `totalProfit`, `soldProductQuantity`, `salesCount` (decimal/int)
- `ProductSummaryStatisticsResponse` — `totalProductCount`, `totalProductCostValue`, `totalProductSaleValue`, `totalInventoryCostValue`, `totalInventorySaleValue`

### Dəyişməyən DTO-lar
- `DashboardDto`, `DashboardStatisticsDto` — köhnə order-based statistikalar (breaking change yoxdur)

## Database / Migration Changes

No database migration required.

## Business Rule Changes

### Günlük satış statistikası
- Interval: cari UTC günü (`SoldAt >= today 00:00 UTC AND SoldAt < tomorrow 00:00 UTC`)
- `totalSaleAmount = SUM(Sale.TotalSaleAmount)` (= SUM(salePrice × quantity))
- `totalProductCost = SUM(Sale.TotalCost)` (= SUM(costPrice × quantity))
- `totalExpenses = SUM(Sale.TotalExpenses)` (satış xərclərinin cəmi)
- `totalProfit = totalSaleAmount - totalProductCost - totalExpenses`
- `soldProductQuantity = SUM(Sale.Quantity)`
- `salesCount = COUNT(sales)`

### Aylıq satış statistikası
- Interval: cari ayın 1-i UTC – bu günün sonu UTC (eyni hesablama qaydaları)

### Məhsul summary statistikası
- Soft deleted məhsullar istisna (global query filter)
- `totalProductCount = COUNT(products)`
- `totalProductSaleValue = SUM(Price.Amount)`
- `totalInventorySaleValue = SUM(Price.Amount × Stock)`
- `totalProductCostValue = SUM(Price.Amount)` — **limitation:** costPrice yoxdur
- `totalInventoryCostValue = SUM(Price.Amount × Stock)` — **limitation:** costPrice yoxdur

### Soft delete
- `IsDeleted = true` olan Sale və Product qeydləri hesablamaya daxil edilmir

## Admin Frontend Impact

Claude Admin paneldə aşağıdakıları etməlidir:

1. **Dashboard / Statistika səhifəsində** köhnə dashboard stat card API integration-larını yeni endpoint ilə əvəz et:
   - `GET /api/dashboard/statistics` çağır
   - Response model: `DashboardStatisticsResponse`

2. **Günlük satış statistikalarını göstər** (`dailySales`):
   - Ümumi satış məbləği (`totalSaleAmount`)
   - Ümumi qazanc (`totalProfit`)
   - Ümumi xərc (`totalExpenses`)
   - Ümumi məhsul alış dəyəri (`totalProductCost`)
   - Satılan məhsul sayı (`soldProductQuantity`)
   - Satış sayı (`salesCount`)

3. **Aylıq satış statistikalarını göstər** (`monthlySales`):
   - Eyni 6 sahə

4. **Məhsul summary statistikalarını göstər** (`productSummary`):
   - Toplam məhsul sayı (`totalProductCount`)
   - Toplam alış/original qiymət (`totalProductCostValue`) — **qeyd:** hazırda Price ilə eyni dəyər
   - Toplam satış qiyməti (`totalProductSaleValue`)
   - Stokla toplam alış dəyəri (`totalInventoryCostValue`)
   - Stokla toplam satış dəyəri (`totalInventorySaleValue`)

5. Köhnə order-based stat card-ları (`DashboardDto.statistics.totalRevenue` və s.) Statistika səhifəsində yeni satış statistikaları ilə əvəz et (və ya ayrıca bölmədə saxla — dizayn qərarı).

6. Loading, empty və error state-ləri saxla.

7. API types/service modellərini yenilə (`DashboardStatisticsResponse`, `SalesStatisticsResponse`, `ProductSummaryStatisticsResponse`).

8. `GET /api/dashboard` artıq auth tələb edir — JWT header göndərilməlidir.

9. Dashboard səhifəsini Playwright MCP ilə test et.

## User Frontend Impact

No User frontend change required.

User frontend dashboard statistikalarını istifadə etmir; yalnız Admin panel təsirlənir.

## OpenAPI

- contracts/openapi.json updated: **yes**
- contracts/openapi.diff.md updated: **yes**

## Test Result

- Backend build: **SUCCESS** (0 errors)
- Backend tests: **Not run** (test layihəsi yoxdur)
- Manual API test: **Not run** (DB/server bu sessiyada test edilməyib)
- Known issues:
  - Product cost price field yoxdur — cost summary dəyərləri Price əsasında hesablanır
  - Tarix filtrləri UTC-dir (local timezone deyil)

## Frontend Auto Sync Instruction

Claude must read this file immediately.
Claude must implement the required Admin Dashboard frontend changes.
Claude must not edit backend code.
Claude must run frontend build/typecheck if available.
Claude must test Dashboard page with Playwright MCP if available.
Claude must write result to:

- ai/handoff/frontend-sync-result.md
- ai/reports/latest-ui-report.md

## Run Frontend Auto Sync

After updating backend-change-result.md, run this command:

cmd /c "C:\Users\Lenovo LEGION\Documents\ElectronicsNumberOne.UI\sync-front.cmd"
