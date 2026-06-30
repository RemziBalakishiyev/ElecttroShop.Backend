# Backend Change Result

## Summary

Admin Categories səhifəsi ilə Add Product modalındakı category lookup arasında kateqoriya sayı uyğunsuzluğu aradan qaldırıldı. Hər iki endpoint indi eyni aktiv kateqoriya görünürlük məntiqindən istifadə edir; default olaraq bütün aktiv (silinməmiş) kateqoriyalar qaytarılır.

## Root Cause

Hər iki endpoint (`GET /api/categories` və `GET /api/categories/lookup`) `IncludeAll=false` default ilə işləyirdi. Bu halda repository `ParentId == null` filter-i tətbiq edirdi — yalnız **root** kateqoriyalar qaytarılırdı.

- Admin Categories page: `GET /api/categories?page=1&pageSize=10` — `IncludeAll` göndərilmirdi → yalnız 2 root kateqoriya
- Add Product dropdown: `GET /api/categories/lookup?IncludeAll=true` — bütün 7 aktiv kateqoriya

`IsDeleted` filter-i hər iki yerdə eyni idi (EF global query filter + explicit `!IsDeleted`). `IsActive` sahəsi Category entity-də yoxdur. Fərq yalnız default `IncludeAll` və ona bağlı parent/root filterində idi.

## Changed Endpoints

### GET /api/categories

* **Old behavior:** Default `IncludeAll=false` → yalnız root kateqoriyalar (`ParentId == null`), `IsDeleted=false`
* **New behavior:** Default `IncludeAll=true` → bütün aktiv kateqoriyalar (root + child), `IsDeleted=false`
* **Query params:** `Page`, `PageSize`, `SearchTerm`, `ParentId`, `IncludeChildren`, `IncludeAll` (default indi `true`)
* **Response:** Dəyişməyib — səhifələnmiş `CategoryDto` (`id`, `name`, `slug`, `parentId`, `parentName`, `discountPercent`, `createdAt`)
* **Validation/filtering rules:**
  - `IsDeleted=false` (həmişə)
  - `ParentId` verildikdə: yalnız həmin parent-ın uşaqları
  - `IncludeAll=false` explicit verildikdə və `ParentId` yoxdursa: yalnız root kateqoriyalar
  - `SearchTerm` ad üzrə filter (dəyişməyib)
  - Pagination dəyişməyib

### GET /api/categories/lookup

* **Old behavior:** Default `includeAll=false` → yalnız root kateqoriyalar
* **New behavior:** Default `includeAll=true` → bütün aktiv kateqoriyalar; list endpoint ilə eyni filter məntiqi
* **Query params:** `includeAll` (default indi `true`), `parentId`
* **Response:** Dəyişməyib — `LookupResponse` (`items[]` with `key`/`value`, `cachedAt`, `cacheKey`)
* **Validation/filtering rules:** List endpoint ilə eyni `ApplyCategoryVisibilityFilters` helper-i

## Changed Models / DTOs

No DTO changes. Yalnız query param default dəyərləri və repository filter məntiqi yeniləndi.

## Database / Migration Changes

No migration required. Schema dəyişməyib.

## Business Rule Changes

* Categories management page və lookup eyni aktiv kateqoriya görünürlük məntiqindən istifadə edir.
* Default: bütün aktiv (silinməmiş) kateqoriyalar.
* Parent/child filter yalnız explicit param ilə:
  - `ParentId` → müəyyən parent-ın uşaqları
  - `IncludeAll=false` / `includeAll=false` → yalnız root kateqoriyalar
* Pagination yalnız `GET /api/categories`-də tətbiq olunur.
* Lookup sadələşdirilmiş DTO qaytarır, amma eyni valid kateqoriya setini istifadə edir.

## Admin Frontend Impact

* Response shape dəyişməyib — **frontend kod dəyişikliyi tələb olunmaya bilər**.
* Yoxlama:
  - Categories page API inteqrasiyası (`GET /api/categories?page=1&pageSize=10`)
  - Add Product category dropdown (`GET /api/categories/lookup`)
* Backend restart/redeploy sonrası Categories page-də 7 kateqoriya görünməlidir (əvvəl 2 root).
* `IncludeAll=true` artıq lookup üçün məcburi deyil (default artıq `true`), amma göndərilsə də problem yaratmır.
* Root-only siyahı lazım olsa: `?IncludeAll=false` əlavə edin.

## User Frontend Impact

No User frontend change required (bu endpoint-lər əsasən Admin üçündür). Əgər User frontend root-only default-a güvənirdisə, `IncludeAll=false` explicit göndərməlidir.

## OpenAPI

* contracts/openapi.json updated: **yes** (`IncludeAll` / `includeAll` default `true`, description əlavə edildi)
* contracts/openapi.diff.md updated: **yes**

## Test Result

* **Backend build:** Application və Persistence layları uğurla compile oldu. Full solution build IIS Express file lock səbəbindən WebApi copy addımında uğursuz oldu (IIS Express/VS işləyir). Kod xətası yoxdur.
* **Backend tests:** Test layihəsi yoxdur.
* **Manual API test:** Backend restart lazımdır. Sonra yoxlayın:
  - `GET /api/categories?page=1&pageSize=10` → `totalCount` lookup ilə eyni olmalıdır (məs. 7)
  - `GET /api/categories/lookup` → eyni kateqoriya ID-ləri
  - `GET /api/categories?IncludeAll=false` → yalnız root kateqoriyalar (köhnə default davranış)
* **Known issues:** IIS Express işləyərkən full rebuild file lock verə bilər; dəyişikliklərin tətbiqi üçün API restart edin.

## Frontend Auto Sync Instruction

Claude must read this file.

Claude must update frontend only if the backend response shape, endpoint usage, or query params changed.

**Response shape dəyişməyib** — əsasən retest kifayətdir.

Claude must not edit backend code.

Claude must retest:

* Admin Categories page — bütün aktiv kateqoriyalar görünməlidir
* Add Product category dropdown — eyni kateqoriya seti
* Product creation flow category selection
