# OpenAPI Diff — Cloudinary Image Storage Migration

## Date
2026-07-06

## Summary
Product image uploads moved from local filesystem to Cloudinary. Response DTOs now return direct Cloudinary HTTPS URLs in `imageUrl` / `primaryImageUrl` fields when available.

## Changed Endpoints

### POST /api/products/{productId}/image
- Response `ProductDto.primaryImageUrl` and `ProductDto.images[].imageUrl` may now be absolute Cloudinary URLs (`https://res.cloudinary.com/...`) instead of `/api/images/{guid}` paths.

### GET /api/images/{imageId}
- May return **302 Redirect** to Cloudinary URL when `ProductImages.ImageUrl` is populated in DB.
- Legacy local files still served as binary stream when present on disk.

### GET /api/admin/debug/image/{id}
**Response extended with:**
| Field | Type | Description |
|-------|------|-------------|
| imageUrl | string? | Cloudinary secure URL from DB |
| publicId | string? | Cloudinary public_id |
| imagePath | string? | Legacy local path |
| storageProvider | string? | e.g. `Cloudinary` |

## Changed Models

### ProductImage (database entity — reflected in API responses via ProductImageDto)
New nullable fields stored server-side; exposed indirectly via resolved `imageUrl` in responses.

## New Configuration (not in OpenAPI)
Environment variables required in production:
- `Cloudinary__CloudName`
- `Cloudinary__ApiKey`
- `Cloudinary__ApiSecret`
- `Cloudinary__Folder` (default: `smartal/products`)

## Breaking Changes
None — backward compatible. Old images without `ImageUrl` continue using `/api/images/{id}` resolution.

## Notes
- Upload validation: max 5MB; content types `image/jpeg`, `image/png`, `image/webp`, `image/gif`
- `contracts/openapi.json` full regeneration pending next clean WebApi build
