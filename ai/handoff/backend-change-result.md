# Backend Change Result

## Summary
Detallı application loglama sistemi əlavə edildi. Loglar artıq yalnız konsolda deyil, PostgreSQL-də `AppLogs` cədvəlində də saxlanılır. HTTP request, MediatR command/query, validasiya xətaları və unhandled exception-lar geniş kontekstlə loglanır. Admin istifadəçilər logları API vasitəsilə oxuya bilər.

## Changed Endpoints

### GET /api/admin/logs
- **Method:** GET
- **URL:** `/api/admin/logs`
- **Auth:** JWT Bearer, **Admin role tələb olunur**
- **Old behavior:** Endpoint yox idi
- **New behavior:** Paginated application log siyahısı qaytarır
- **Query params:**
  - `page` (default: 1)
  - `pageSize` (default: 20, max: 100)
  - `level` (Information, Warning, Error, ...)
  - `eventType` (HttpRequest, MediatR, Validation, Exception, Application, ...)
  - `correlationId`
  - `userId` (Guid)
  - `search` (message, exception, path, sourceContext üzrə)
  - `dateFrom`, `dateTo` (UTC)
- **Response body:** `PagedResult<AppLogDto>`
- **Validation rules:** page/pageSize standart pagination
- **Error responses:** 401 Unauthorized, 403 Forbidden (non-admin)
- **Status/enum changes:** yoxdur

## Changed Models / DTOs

### AppLogDto (yeni)
- `id`, `timestampUtc`, `level`, `message`, `exception`
- `sourceContext`, `eventType`, `correlationId`
- `userId`, `userEmail`
- `requestPath`, `requestMethod`, `queryString`, `requestBody`
- `responseStatusCode`, `elapsedMilliseconds`
- `clientIp`, `userAgent`, `machineName`, `propertiesJson`

## Database / Business Rule Changes

- Yeni cədvəl: **`AppLogs`**
- Migration: `20260705184000_AddAppLogs`
- Log yazma background queue ilə batch (50 entry / 2 saniyə) edilir
- Sensitive field-lər (password, token, otp, refreshToken və s.) avtomatik `***REDACTED***` olaraq maskalanır
- `/health`, `/swagger` request-ləri loglanmır

## Frontend Impact

### Admin frontend
- **Yeni səhifə tövsiyəsi:** Logs / Audit ekranı
- `GET /api/admin/logs` endpoint-inə inteqrasiya
- Filter: level, eventType, tarix aralığı, search, correlationId
- Cədvəldə: timestamp, level, eventType, message, user, path, statusCode, elapsedMs
- Detail modal: exception, requestBody, propertiesJson

### User frontend
- **No frontend change required**

## OpenAPI
- contracts/openapi.json updated: yes
- contracts/openapi.diff.md updated: yes

## Test Result
- Backend build: uğurlu (`dotnet build -o obj/webapi-build-temp`)
- Backend tests: test layihəsi yoxdur
- Manual API test:
  1. Migration tətbiq et
  2. API-ni işə sal
  3. Bir neçə endpoint çağır
  4. `AppLogs` cədvəlində və ya `GET /api/admin/logs` ilə yoxla
- Known issues: IIS Express işləyirsə default build output lock ola bilər

## Security Notes
- Log endpoint yalnız Admin roluna açıqdır
- Parol, token, OTP və oxşar field-lər log payload-larından maskalanır
- Request body yalnız JSON/text content type-lar üçün loglanır; multipart fayl upload body-si loglanmır
