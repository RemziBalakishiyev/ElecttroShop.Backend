# Backend Change Result

## Summary

Production `ERR_BLOCKED_BY_ORB` fix: image endpoint artıq **heç vaxt JSON/problem+json qaytarmır**. Fayl varsa real `image/*` stream, yoxdursa boş **404** + `Content-Type: image/jpeg` (ORB bloklamasın).

**ORB səbəbi:** `[ApiController]` + `NotFound()` → `application/problem+json` → Chrome `<img>` üçün bloklayır.

**404 səbəbi (production):** Render diskində fayl yoxdur (ephemeral storage). DB-də `ProductImages.ImageId` qalır.

## Changed Endpoints

### GET /api/images/{imageId}
- DB-də `ProductImage` record axtarılır (`ImageId` ilə)
- Fiziki fayl: `wwwroot/images/products/{imageId}.{ext}`
- **200:** `File(stream, image/jpeg|png|webp|gif)` — JSON yox
- **404:** boş body, `Content-Type: image/jpeg` — JSON yox
- Log (404): `ImageId`, `DbPath`, `StorageBasePath`, `PhysicalPath`

### Static alternative (UseStaticFiles aktiv)
Fayl diskdə varsa API DTO-larda URL:
`https://api.smartal.net/images/products/{imageId}.jpg`

## Frontend Impact (REQUIRED)

### Env
```
VITE_API_BASE_URL=https://api.smartal.net/api
VITE_ASSET_BASE_URL=https://api.smartal.net
```

### `src/utils/imageUrl.ts`
```typescript
const PLACEHOLDER = '/images/placeholder-product.png';
const API_BASE = (import.meta.env.VITE_API_BASE_URL ?? '').replace(/\/$/, '');
const ASSET_BASE = (import.meta.env.VITE_ASSET_BASE_URL ?? '').replace(/\/$/, '');
const GUID_REGEX = /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

export function resolveImageUrl(value?: string | null): string {
  if (!value?.trim()) return PLACEHOLDER;
  const trimmed = value.trim();
  if (/^https?:\/\//i.test(trimmed)) return trimmed;
  if (GUID_REGEX.test(trimmed)) return `${API_BASE}/images/${trimmed}`;
  if (trimmed.startsWith('/images/') || trimmed.startsWith('/uploads/')) return `${ASSET_BASE}${trimmed}`;
  if (trimmed.startsWith('images/') || trimmed.startsWith('uploads/')) return `${ASSET_BASE}/${trimmed}`;
  if (trimmed.startsWith('/api/images/')) return `${ASSET_BASE}${trimmed}`;
  if (trimmed.startsWith('/')) return `${ASSET_BASE}${trimmed}`;
  return `${ASSET_BASE}/${trimmed}`;
}
```

Bütün `<img src={...}>` → `src={resolveImageUrl(...)}`

**Admin:** product table, form preview, dashboard  
**User:** home, products, detail, cart

## OpenAPI
- contracts/openapi.json: pending export after deploy
- contracts/openapi.diff.md: updated

## Test Result
- Backend build: **passed**
- Local test: `GET /api/images/{existing-id}` → 200 `image/jpeg`; missing id → 404 `image/jpeg` (no JSON)
- Frontend build: not run (repos not in workspace)

## Production test
1. Deploy backend
2. Browser: `https://api.smartal.net/api/images/{id}` — real image görünməlidir
3. `GET /api/admin/debug/image/{id}` — `fileExists: false` olarsa şəkilləri yenidən upload et

## Run Frontend Auto Sync
cmd /c "C:\Users\Lenovo LEGION\Documents\ElectronicsNumberOne.UI\sync-front.cmd"
