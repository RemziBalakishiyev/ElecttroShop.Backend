# Backend Change Result

## Summary
Deploy/start zamanı migration avtomatik tətbiq olunur, seed data isə production deploy-da işləmir. Migration default olaraq aktivdir; seed yalnız Development mühitində (və ya `SEED_ON_STARTUP=true` olduqda) icra olunur.

## Changed Endpoints
Heç bir API endpoint dəyişməyib.

## Changed Models / DTOs
Yoxdur.

## Database / Business Rule Changes

### Startup davranışı (Program.cs)

| Mühit | Migration | Seed |
|-------|-----------|------|
| Production (deploy) | ✅ Avtomatik (`MIGRATE_ON_STARTUP` default: `true`) | ❌ Deaktiv |
| Development (local) | ✅ Avtomatik | ✅ Aktiv (`SEED_ON_STARTUP` default: `true`) |

### Konfiqurasiya

- `MIGRATE_ON_STARTUP` — default `true`. `false` olarsa migration skip edilir.
- `SEED_ON_STARTUP` — default `false` (production). Development-da default `true`.

### appsettings.Production.json
```json
"MIGRATE_ON_STARTUP": true,
"SEED_ON_STARTUP": false
```

## Frontend Impact

### Admin frontend
No frontend change required.

### User frontend
No frontend change required.

## OpenAPI
- contracts/openapi.json updated: no
- contracts/openapi.diff.md updated: no

## Test Result
- Backend build: pending
- Backend tests: test layihəsi yoxdur
- Manual API test:
  1. Production mühitində API start et
  2. Pending migration varsa avtomatik tətbiq olunmalıdır
  3. Seed data əlavə olunmamalıdır (mövcud Users/Categories/Products sayı dəyişməməlidir)
- Known issues: yoxdur

## Security Notes
Production-da test admin/agent istifadəçiləri avtomatik yaradılmır.
