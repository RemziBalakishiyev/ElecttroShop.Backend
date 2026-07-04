# Backend Change Result

## Summary

**Latest change:** Production product image 404-ləri üçün backend image pipeline gücləndirildi — storage path content root-a görə resolve olunur, `UseStaticFiles` aktivdir, 404-lərdə physical path loglanır, admin debug endpoint-ləri əlavə olundu, `PUBLIC_BASE_URL` ilə DTO-larda absolute image URL qaytarıla bilər.

**Kök səbəb (production images):** Render Docker konteynerində `wwwroot/images/products` **persistent deyil**. DB-də `ProductImages.ImageId` olsa belə redeploy/restart-dan sonra fayl diskdə yoxdursa `GET /api/images/{id}` 404 qaytarır.

**Storage path:** `wwwroot/images/products/{imageId}.{ext}`

---

## Changed Endpoints — Image handling

### GET /api/images/{imageId}
- Method: GET | Auth: Anonymous
- New behavior: 404 log-da `SearchedPath` və `BasePath`; fayl varsa düzgün content-type

### GET /api/admin/debug/uploads (NEW)
- Auth: Bearer | Response: storage path, file count, first 50 files

### GET /api/admin/debug/image/{id} (NEW)
- Auth: Bearer | Response: DB record, physical path, fileExists, publicUrl

---

## Changed Endpoints — Dashboard (previous, still active)

### GET /api/dashboard/statistics (NEW)
- Auth: JWT | Response: `DashboardStatisticsResponse` (daily/monthly sales + product summary)

### GET /api/dashboard / GET /api/dashboard/chart
- Auth: `[Authorize]` enabled | Response unchanged

---

## Changed Models / DTOs

**Images (when `PUBLIC_BASE_URL` set):** `primaryImageUrl`, `imageUrl` fields may become absolute URLs.

**New:** `UploadsDebugResponse`, `UploadDebugFileDto`, `ImageDebugResponse`

**Dashboard:** `DashboardStatisticsResponse`, `SalesStatisticsResponse`, `ProductSummaryStatisticsResponse`

---

## Frontend Impact

### Admin (REQUIRED — images)
1. Env: `VITE_API_BASE_URL=https://api.smartal.net/api`, `VITE_ASSET_BASE_URL=https://api.smartal.net`
2. Create `src/utils/imageUrl.ts` — see `resolveImageUrl()` in prior handoff
3. Use helper on all `<img src={...}>` (products, categories, banners, dashboard)

### Admin (REQUIRED — dashboard, if not done)
- Integrate `GET /api/dashboard/statistics` on Statistika page
- Send JWT on all dashboard endpoints

### User (REQUIRED — images)
- Same `imageUrl.ts` helper and env vars on home/products/detail/cart

---

## OpenAPI
- contracts/openapi.json updated: yes
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: passed
- Backend tests: not run
- Manual: deploy → `GET /api/admin/debug/uploads` → `GET /api/admin/debug/image/{id}` → re-upload if `fileExists: false`

## Known issues
- Render local disk ephemeral — production images may need re-upload
- Product costPrice field absent — dashboard cost values use Price.Amount

## Run Frontend Auto Sync

cmd /c "C:\Users\Lenovo LEGION\Documents\ElectronicsNumberOne.UI\sync-front.cmd"
