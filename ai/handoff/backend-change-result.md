# Backend Change Result

## Summary
Local debug (Development) mühiti də production PostgreSQL DB istifadə edir. Seed startup-da deaktivdir ki, prod məlumatları pozulmasın.

## Changed Endpoints
Yoxdur.

## Database / Business Rule Changes
- `launchSettings.json`, `appsettings.Development.json`, `appsettings.Development.example.json` — prod connection string
- `SEED_ON_STARTUP=false` debug profillərində (prod DB-yə seed yazılmasın)

## Frontend Impact
No frontend change required.

## OpenAPI
- contracts/openapi.json updated: no
- contracts/openapi.diff.md updated: no

## Security Notes
- Prod DB parolu konfiq fayllarındadır; public repo riski var.
- Lokal debug birbaşa production DB-yə yazır — ehtiyatlı olun.
