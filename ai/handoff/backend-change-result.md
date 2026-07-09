# Backend Change Result

## Summary
Məhsul şəkli silmə funksionallığı hazırdır və təkmilləşdirildi. Admin panel məhsul şəklini siləndə həm PostgreSQL bazasından (`ProductImages` cədvəli), həm də Cloudinary storage-dan (varsa) silinir. Primary şəkil silinərsə, qalan şəkillərdən birincisi avtomatik primary olur.

## Changed Endpoints

### DELETE /api/Products/{productId}/images/{imageId}
- **Method:** DELETE
- **URL:** `/api/Products/{productId}/images/{imageId}`
- **Auth:** JWT Bearer token (Admin istifadəçi)
- **Old behavior:** Endpoint mövcud idi; Cloudinary silmə yalnız `PublicId` olduqda işləyirdi; primary silinəndə avtomatik yenilənmirdi
- **New behavior:**
  - Şəkil DB-dən silinir
  - Cloudinary-dən silinir (`PublicId` və ya `StorageProvider=Cloudinary` / Cloudinary URL fallback ilə)
  - Köhnə local fayl varsa diskdən silinir
  - Primary şəkil silinərsə, `DisplayOrder`-a görə növbəti şəkil primary olur
- **Request body:** yoxdur
- **Response body:** `200 OK` — boş body (uğurlu silmə)
- **Validation rules:**
  - `productId` — valid UUID, mövcud məhsul
  - `imageId` — valid UUID, həmin məhsula aid şəkil
- **Error responses:**
  - `404 Product.NotFound` — məhsul tapılmadı
  - `404 ProductImage.NotFound` — şəkil bu məhsula aid deyil
  - `401` — token yoxdur və ya etibarsızdır
- **Status/enum changes:** yoxdur

## Changed Models / DTOs
- Yeni error kodu: `ProductImage.NotFound`
- `ProductImageDto` dəyişməyib — silmə üçün `imageId` field istifadə olunur

```json
{
  "id": "guid",
  "imageId": "guid",
  "imageUrl": "https://res.cloudinary.com/...",
  "displayOrder": 0,
  "isPrimary": true
}
```

## Database / Business Rule Changes
- Migration yoxdur
- `ProductImages` cədvəlindən hard delete (cascade)
- Primary şəkil silinəndə domain `Product.RemoveImage` avtomatik növbəti primary təyin edir

## Frontend Impact

### Admin
**Tələb olunan dəyişikliklər:**

1. **Məhsul redaktə / şəkil idarəetmə UI-da hər şəkil üçün "Sil" düyməsi əlavə edin**
   - Silmədən əvvəl confirm dialog göstərin: "Bu şəkli silmək istədiyinizə əminsiniz?"
   - Primary şəkil silinəndə xəbərdarlıq əlavə edin (backend avtomatik yeni primary təyin edir)

2. **API çağırışı:**
   ```
   DELETE /api/Products/{productId}/images/{imageId}
   Authorization: Bearer {token}
   ```
   - `productId` — cari məhsulun ID-si
   - `imageId` — `ProductImageDto.imageId` (NOT `ProductImageDto.id`)

3. **Uğurlu silmədən sonra:**
   - Local state-dən şəkli silin VƏ ya məhsulu yenidən yükləyin (`GET /api/Products/{id}`)
   - `primaryImageUrl` və `images` listini yeniləyin

4. **Xəta idarəetməsi:**
   - `404` — "Şəkil tapılmadı" mesajı
   - `401` — login səhifəsinə yönləndirmə

5. **Nümunə TypeScript funksiya:**
   ```typescript
   async function deleteProductImage(productId: string, imageId: string): Promise<void> {
     const response = await fetch(
       `/api/Products/${productId}/images/${imageId}`,
       { method: 'DELETE', headers: { Authorization: `Bearer ${token}` } }
     );
     if (!response.ok) throw new Error('Şəkil silinmədi');
   }
   ```

6. **UI tövsiyələri:**
   - Silmə zamanı loading state (spinner/disable button)
   - Son şəkil silinəndə xəbərdarlıq (məhsul şəkilsiz qalacaq)
   - Primary badge-i silmədən sonra avtomatik yenilənməlidir

### User
No frontend change required — User frontend yalnız oxuyur.

## OpenAPI
- contracts/openapi.json updated: yes
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: success (0 errors)
- Backend tests: test layihəsi yoxdur
- Manual API test:
  1. Admin token ilə `GET /api/Products/{id}` — şəkilləri qeyd edin
  2. `DELETE /api/Products/{productId}/images/{imageId}` çağırın
  3. `GET /api/Products/{id}` — şəkil silinmiş olmalıdır
  4. Cloudinary dashboard-da fayl silinmiş olmalıdır
- Known issues: Cloudinary/local silmə uğursuz olsa belə DB silinməsi davam edir (best-effort cleanup)
