# Backend Change Result

## Summary
`cannot drop system column "xmin"` xətası düzəldildi. PostgreSQL-də `xmin` sistem sütunudur — silinə və ya əlavə edilə bilməz. Migration no-op edildi; concurrency düzəlişi `AggregateRootConfiguration`-da `RowVersion` + `IsRowVersion()` ilə qalır.

## Changed Endpoints

### PUT /api/products/{id}
- **Auth:** Admin
- **New behavior:** `rowVersion` köhnədirsə 409; düzgün `xmin` map ilə concurrency
- **Request:** `rowVersion` GET cavabından göndərilməlidir

## Database / Business Rule Changes
- `20260707191500_DropPhysicalXminColumns` — schema dəyişikliyi yoxdur (no-op)
- `20260523120000_FixRowVersionUsePostgresXmin` — yalnız köhnə bytea `RowVersion` silir, fiziki `xmin` əlavə etmir

## Frontend Impact
- **Admin:** Update zamanı `rowVersion` göndərin; 409 alınarsa məhsulu yenidən yükləyin
- **User:** No frontend change required

## OpenAPI
- contracts/openapi.json updated: no
- contracts/openapi.diff.md updated: no

## Test Result
- Backend build: run after IIS Express stop
- Migration: `dotnet ef database update` — `DropPhysicalXminColumns` indi xətasız keçməlidir
