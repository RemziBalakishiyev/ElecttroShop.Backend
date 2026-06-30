# OpenAPI diff — Categories List/Lookup Consistency Fix

**Date:** 2026-06-30

## Summary

`GET /api/categories` və `GET /api/categories/lookup` endpoint-lərində `IncludeAll` query parametrinin default dəyəri `false`-dan `true`-ya dəyişdirildi. Response shape dəyişməyib.

## Changed query parameters

### GET /api/categories
- **IncludeAll** — default: `false` → `true`
- **Description:** `false` olduqda yalnız root kateqoriyalar; default: bütün aktiv kateqoriyalar

### GET /api/categories/lookup
- **includeAll** — default: `false` → `true`
- **Description:** `false` olduqda yalnız root kateqoriyalar; default: bütün aktiv kateqoriyalar

## Breaking changes

- **Behavioral (low risk):** Caller-lər `IncludeAll`/`includeAll` göndərmədikdə əvvəl yalnız root kateqoriyalar gəlirdi, indi bütün aktiv kateqoriyalar gəlir.
- Root-only siyahı lazımdırsa: `?IncludeAll=false` (management) və ya `?includeAll=false` (lookup) explicit göndərilməlidir.
- Response/request DTO shape dəyişməyib.

## Frontend note

- Admin Categories page və Add Product dropdown eyni kateqoriya setini görməlidir — əlavə frontend dəyişikliyi tələb olunmaya bilər.
- User frontend root-only default-a güvənirdisə, `IncludeAll=false` əlavə edin.
