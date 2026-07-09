# OpenAPI Diff — Product Image Delete

## Date
2026-07-09

## Summary
Məhsul şəkli silmə endpoint-i sənədləşdirildi və Cloudinary fallback silmə davranışı təkmilləşdirildi.

## Changed Endpoints

### DELETE /api/Products/{productId}/images/{imageId}
- **Summary:** Məhsuldan şəkil silir (bazadan və Cloudinary-dən)
- **Auth:** JWT Bearer token (Admin panel)
- **Path params:**
  - `productId` (uuid, required) — məhsul ID
  - `imageId` (uuid, required) — silinəcək şəklin `imageId` dəyəri (`ProductImageDto.imageId`)
- **Request body:** yoxdur
- **Success:** `200 OK` (boş body)
- **Errors:**
  - `404` — məhsul və ya şəkil tapılmadı (`Product.NotFound`, `ProductImage.NotFound`)
  - `401` — autentifikasiya tələb olunur

## Backend Behavior
1. Məhsul və şəkil DB-dən yoxlanılır
2. Cloudinary-də silinir (`PublicId` varsa; yoxdursa `smartal/products/{imageId}` fallback)
3. Köhnə local fayl varsa diskdən silinir
4. `ProductImages` cədvəlindən sətir silinir
5. Silinən şəkil primary idisə, qalan şəkillərdən birincisi avtomatik primary olur

## Breaking Changes
None — endpoint əvvəldən mövcud idi, davranış geriyə uyğundur.
