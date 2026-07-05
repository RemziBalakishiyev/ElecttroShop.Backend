# Backend Change Result

## Summary
Məhsul şəkil yükləməsi Render lokal filesystem-dən Cloudinary object storage-a keçirildi. Yeni upload-lar Cloudinary-də saxlanır; DB-də `ImageUrl`, `PublicId`, `StorageProvider` və əlaqəli metadata yazılır. Köhnə lokal şəkillər geriyə uyğunluq üçün `/api/images/{id}` vasitəsilə oxuna bilər.

## Changed Endpoints

### POST /api/products/{productId}/image
- **Method:** POST
- **URL:** `/api/products/{productId}/image`
- **Auth:** JWT (controller səviyyəsində)
- **Old behavior:** Fayl `wwwroot/images/products`-a yazılırdı, yalnız `ImageId` DB-də saxlanırdı
- **New behavior:** Fayl Cloudinary-ə yüklənir; DB-də `ImageUrl` (secure URL), `PublicId`, `FileName`, `ContentType`, `Size`, `StorageProvider=Cloudinary` yazılır
- **Request body:** `multipart/form-data` — `file`
- **Response body:** `ProductDto` — `primaryImageUrl` və `images[].imageUrl` birbaşa Cloudinary HTTPS URL qaytarır
- **Validation:** JPEG/PNG/WebP/GIF, max 5MB
- **Error responses:** 400 validation/upload xətası; Cloudinary konfiqurasiya yoxdursa upload uğursuz

### POST /api/images/upload
- **Old behavior:** Lokal diskə yazırdı
- **New behavior:** Cloudinary-ə yükləyir; `{ imageId }` qaytarır (public_id son seqmentindən)

### GET /api/images/{imageId}
- **Old behavior:** Yalnız lokal fayl stream qaytarırdı
- **New behavior:** DB-də `ImageUrl` varsa **302 Redirect** Cloudinary URL-ə; lokal fayl varsa əvvəlki stream məntiqi; yoxdursa 404 + log

### DELETE /api/products/{productId}/images/{imageId}
- **New behavior:** `PublicId` varsa Cloudinary-dən silir, sonra DB record silir; köhnə lokal şəkil üçün disk silməsi fallback

### GET /api/admin/debug/image/{id}
- **New fields in response:** `imageUrl`, `publicId`, `imagePath`, `storageProvider`, `imageRecordFound`

## Changed Models / DTOs

### ProductImage (entity / DB)
Yeni sütunlar:
- `ImageUrl` (nullable string)
- `PublicId` (nullable string)
- `ImagePath` (nullable string, legacy fallback)
- `FileName` (nullable string)
- `ContentType` (nullable string)
- `Size` (nullable long)
- `StorageProvider` (string, default `Cloudinary`)

### ProductImageDto / ProductDto
- `imageUrl` / `primaryImageUrl` — əvvəlcə DB `ImageUrl`, sonra `ImagePath` fallback, sonra `/api/images/{id}`

### ImageUploadResultDto (internal)
- `Url`, `SecureUrl`, `PublicId`, `FileName`, `ContentType`, `Size`, `StorageProvider`

### ImageDebugResponse
- `imageUrl`, `publicId`, `imagePath`, `storageProvider` əlavə edildi

## Database / Business Rule Changes
- Migration: `20260706040000_AddCloudinaryFieldsToProductImages`
- Yeni upload-lar yalnız Cloudinary istifadə edir
- Product update zamanı silinən şəkillər Cloudinary-dən də silinir
- Köhnə `ImageId` və lokal path-lər qorunur (backward compatible)

## Frontend Impact

### Admin frontend
- Məhsul şəkil upload axını eyni endpoint-i istifadə edir (`POST /api/products/{id}/image`)
- **`images[].imageUrl` və `primaryImageUrl` artıq birbaşa Cloudinary HTTPS URL ola bilər** — `<img src={imageUrl}>` üçün əlavə transform lazım deyil
- Köhnə məhsullarda URL hələ `/api/images/{guid}` formatında qala bilər; hər iki hal işləməlidir
- Cloudinary URL-lər cross-origin `<img>` üçün uyğundur; CORS/API proxy tələb etmir

### User frontend
- Eyni: `primaryImageUrl` / `imageUrl` birbaşa Cloudinary URL ola bilər
- `VITE_ASSET_BASE_URL` prepend etməyin Cloudinary absolute URL-lər üçün
- URL `https://res.cloudinary.com/...` ilə başlayırsa olduğu kimi istifadə edin

## Render Environment Variables (Production)
```
Cloudinary__CloudName=<cloud_name>
Cloudinary__ApiKey=<api_key>
Cloudinary__ApiSecret=<secret>
Cloudinary__Folder=smartal/products
```
Secret-lər yalnız Render Environment Variables-da olmalıdır — repo-ya yazılmır.

## OpenAPI
- contracts/openapi.json updated: no (IIS Express file lock səbəbindən tam export edilmədi; debug response sahələri genişləndi)
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: Application + Persistence + migration **SUCCESS**
- WebApi build: IIS Express/Visual Studio file lock səbəbindən copy addımı uğursuz (kod compile olunur)
- Backend tests: layihədə test projekt yoxdur
- Manual API test:
  1. Render-də Cloudinary env-ləri təyin edin
  2. Migration tətbiq edin (`MIGRATE_ON_STARTUP=true` və ya manual)
  3. `POST /api/products/{id}/image` — response-da `primaryImageUrl` Cloudinary URL olmalıdır
  4. `GET /api/admin/debug/image/{imageId}` — `imageUrl`, `publicId`, `storageProvider=Cloudinary`
  5. Redeploy sonrası şəkil URL-i işləməlidir
- Known issues: Lokal dev üçün Cloudinary env-ləri `.env`-də placeholder olmalıdır; konfiq yoxdursa upload xəta verir
