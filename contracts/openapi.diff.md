# OpenAPI diff — Dashboard Statistics + Product image handling

**Date:** 2026-07-05

## Summary

Two backend changes in this release:

1. **Dashboard Statistics API** — new admin sales/product statistics endpoint
2. **Product image handling** — static files, debug endpoints, `PUBLIC_BASE_URL` support

---

## Dashboard Statistics (previous change)

### GET /api/dashboard/statistics (NEW)
- **Auth:** `[Authorize]` — JWT required
- **Response:** `DashboardStatisticsResponse` (`dailySales`, `monthlySales`, `productSummary`)

### GET /api/dashboard / GET /api/dashboard/chart
- **Change:** `[Authorize]` enabled (was commented out)
- **Response:** unchanged

### Business rules
- Date filters use UTC (`SoldAt`)
- Soft deleted sales/products excluded
- **Limitation:** no separate `costPrice` on Product — cost values use `Price.Amount`

---

## Product image handling (this change)

### GET /api/admin/debug/uploads (NEW)
- **Auth:** Bearer (staff)
- **Response:** `UploadsDebugResponse` — web root, storage path, file count, first 50 files

### GET /api/admin/debug/image/{id} (NEW)
- **Auth:** Bearer (staff)
- **Response:** `ImageDebugResponse` — DB record, physical path searched, file exists, public URLs

### GET /api/images/{imageId} / GET /api/images/{imageId}.{extension}
- **Auth:** Anonymous
- **Change:** Improved 404 logging includes searched physical path and base path
- **Note:** Returns file from `wwwroot/images/products/{imageId}.{ext}` when present

### Response model changes (when `PUBLIC_BASE_URL` is set)
Product-related DTO fields may return **absolute** URLs:
- `primaryImageUrl`, `imageUrl` (products, variants, popular, chart data)

Example: `https://api.smartal.net/api/images/{guid}.jpg`

### Configuration
- `PUBLIC_BASE_URL` — e.g. `https://api.smartal.net`
- `ImageStorage__BasePath` — optional, default `wwwroot/images/products`

### Render storage note
Local disk uploads are ephemeral on Render. Missing files cause 404 even when DB has `ImageId`.

## Breaking changes
- None for API route structure
- **Ops:** Set `PUBLIC_BASE_URL` on Render for absolute image URLs
- **Frontend:** Use `resolveImageUrl()` helper for image URLs

## OpenAPI
- `contracts/openapi.json` updated: **yes**
