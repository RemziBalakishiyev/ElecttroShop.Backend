# OpenAPI Diff — Sales Monthly Export

## Date
2026-07-09

## Summary
Satışlar moduluna ay/il üzrə Excel və PDF export endpointləri əlavə edildi.

## New Endpoints

### GET /api/sales/export/excel
- **Summary:** Seçilmiş ay üzrə satış hesabatını Excel formatında export edir
- **Auth:** JWT Bearer token (mövcud `/api/sales` ilə eyni — `[Authorize]`)
- **Query params:**
  - `year` (int, required) — 2000..2100
  - `month` (int, required) — 1..12
- **Success:** `200 OK`
  - Content-Type: `application/vnd.openxmlformats-officedocument.spreadsheetml.sheet`
  - Content-Disposition: `attachment; filename="MAY_AYI_SATIS_2026.xlsx"` (ay adı ASCII safe)
- **Errors:**
  - `400` — etibarsız `year` və ya `month`
  - `401` — autentifikasiya tələb olunur

### GET /api/sales/export/pdf
- **Summary:** Seçilmiş ay üzrə satış hesabatını PDF formatında export edir
- **Auth:** JWT Bearer token
- **Query params:** eyni (`year`, `month`)
- **Success:** `200 OK`
  - Content-Type: `application/pdf`
  - Content-Disposition: `attachment; filename="MAY_AYI_SATIS_2026.pdf"`
- **Errors:** eyni (`400`, `401`)

## Data Filter
- Satış tarixi: `SoldAt` (UTC)
- Interval: ayın 1-ci günü 00:00:00 UTC ≤ `SoldAt` < növbəti ayın 1-ci günü 00:00:00 UTC
- Data yoxdursa boş hesabat faylı qaytarılır (xəta yox)

## Breaking Changes
None — yalnız yeni endpointlər əlavə edilib.
