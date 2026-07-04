# OpenAPI diff — Dashboard Statistics API

**Date:** 2026-07-05

## Summary

Admin Dashboard üçün yeni satış və məhsul statistik endpoint-i əlavə olundu. Köhnə `GET /api/dashboard` endpoint-i dəyişməyib (sifariş/məhsul count statistikaları); yeni statistikalar ayrıca endpoint-dədir.

## New endpoints

### GET /api/dashboard/statistics
- **Auth:** `[Authorize]` — JWT tələb olunur (Admin panel pattern)
- **Response:** `DashboardStatisticsResponse`
  - `dailySales`: `SalesStatisticsResponse` — bugünkü satışlar (UTC gün intervalı)
  - `monthlySales`: `SalesStatisticsResponse` — cari ay (ayın 1-dən bu günə qədər, UTC)
  - `productSummary`: `ProductSummaryStatisticsResponse` — sistemdəki məhsullar

## New schemas

- `DashboardStatisticsResponse`
- `SalesStatisticsResponse`: `totalSaleAmount`, `totalProductCost`, `totalExpenses`, `totalProfit`, `soldProductQuantity`, `salesCount`
- `ProductSummaryStatisticsResponse`: `totalProductCount`, `totalProductCostValue`, `totalProductSaleValue`, `totalInventoryCostValue`, `totalInventorySaleValue`

## Changed endpoints

### GET /api/dashboard
- **Change:** `[Authorize]` aktiv edildi (əvvəl comment olunmuşdu)
- **Response:** Dəyişməyib (`DashboardDto` — köhnə order-based statistikalar)

## Business rules

**Sales statistics (daily/monthly):**
```
totalSaleAmount = SUM(salePrice * quantity)  → Sale.TotalSaleAmount
totalProductCost = SUM(costPrice * quantity) → Sale.TotalCost
totalExpenses = SUM(sale expenses)           → Sale.TotalExpenses
totalProfit = totalSaleAmount - totalProductCost - totalExpenses
soldProductQuantity = SUM(quantity)
salesCount = COUNT(sales)
```
- Date filter: `SoldAt` (UTC)
- Soft deleted satışlar istisna (global query filter)

**Product summary:**
```
totalProductCount = COUNT(products)
totalProductSaleValue = SUM(Price.Amount)
totalInventorySaleValue = SUM(Price.Amount * Stock)
```
- **Limitation:** Product entity-də ayrıca `costPrice` yoxdur; `totalProductCostValue` və `totalInventoryCostValue` müvəqqəti olaraq `Price.Amount` əsasında hesablanır (alış qiyməti = satış qiyməti).

## Breaking changes

- **None** for existing dashboard consumers — yeni endpoint əlavədir.
- Admin frontend Dashboard səhifəsi yeni `/api/dashboard/statistics` endpoint-inə keçməlidir.

## Database

No migration required.

## OpenAPI

- `contracts/openapi.json` updated: **yes**
