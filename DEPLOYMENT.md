# Deployment Sənədi

---

## Deployment variantları

| Variant | Status | Təsvir |
|---------|--------|--------|
| Docker (API) | Dockerfile mövcud | Multi-stage .NET 8 build |
| Docker Compose (DB) | Aktiv | Yalnız PostgreSQL |
| IIS | Dəstəklənir | ASP.NET Core hosting |
| CI/CD | **Yoxdur** | Manual deployment |

---

## Docker

### PostgreSQL (docker-compose.yml)

```bash
docker-compose up -d    # Başlat
docker-compose down     # Dayandır
docker-compose logs -f  # Log-lar
```

| Parametr | Dəyər |
|----------|-------|
| Image | postgres:16-alpine |
| Container | electroshop-db |
| Host port | 5434 → 5432 |
| Database | ElectroShopDb |
| User/Password | postgres/postgres |
| Volume | postgres_data |

### API Dockerfile

**Fayl:** `src/ElectroShop.WebApi/Dockerfile`

```
Stage 1: mcr.microsoft.com/dotnet/sdk:8.0 → restore + build
Stage 2: publish (Release)
Stage 3: mcr.microsoft.com/dotnet/aspnet:8.0 → runtime
```

| Parametr | Dəyər |
|----------|-------|
| Exposed ports | 8080 (HTTP), 8081 (HTTPS) |
| Entry point | dotnet ElectroShop.WebApi.dll |

### API-ni Docker ilə işə salmaq

```bash
# Image build
docker build -f src/ElectroShop.WebApi/Dockerfile -t electroshop-api .

# Run (DB ilə birlikdə)
docker run -d \
  -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5434;Database=ElectroShopDb;Username=postgres;Password=postgres" \
  -e Jwt__SecretKey="your-production-secret-key-min-32-chars" \
  -v electroshop-images:/app/wwwroot/images/products \
  electroshop-api
```

**⚠️ Qeyd:** docker-compose.yml-də API service yoxdur — yalnız PostgreSQL var. API ayrıca build/run edilməlidir.

---

## Manual deployment

### 1. Publish

```bash
dotnet publish src/ElectroShop.WebApi/ElectroShop.WebApi.csproj \
  -c Release \
  -o ./publish
```

### 2. Server tələbləri

- .NET 8.0 ASP.NET Core Runtime
- PostgreSQL 16+
- Reverse proxy (Nginx/IIS) — HTTPS üçün

### 3. Environment variables

```bash
export ASPNETCORE_ENVIRONMENT=Production
export ConnectionStrings__DefaultConnection="Host=db-server;Port=5432;Database=ElectroShopDb;Username=app;Password=***"
export Jwt__SecretKey="production-secret-key-at-least-32-characters"
export ImageStorage__BasePath="/var/electroshop/images"
```

### 4. Migration

**Variant A:** Startup-da avtomatik (hazırkı davranış)

**Variant B (tövsiyə):** CI/CD-də ayrıca:

```bash
dotnet ef database update \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi
```

---

## IIS deployment

1. .NET 8 Hosting Bundle quraşdırın
2. IIS-də Application Pool yaradın (No Managed Code)
3. Site yaradın, publish qovluğunu point edin
4. `web.config` avtomatik generasiya olunur
5. Connection string və JWT secret-i environment variable ilə verin

**Qeyd:** `launchSettings.json`-da IIS Express profili mövcuddur, amma production IIS konfiqurasiyası repo-da yoxdur — dəqiqləşdirilməlidir.

---

## Production diqqət edilməli hissələr

### Təhlükəsizlik

| Məsələ | Hazırkı vəziyyət | Tövsiyə |
|--------|------------------|---------|
| JWT Secret | appsettings.json-da hardcoded | Environment variable |
| CORS | AllowAll | Spesifik origin-lər |
| Products/Dashboard auth | Comment edilib | `[Authorize]` aktivləşdir |
| Password hash | SHA256 (salt yoxdur) | bcrypt/Argon2 |
| Swagger | Hər yerdə açıq | Production-da deaktiv et |
| HTTPS | Dev certificate | Reverse proxy TLS |

### Database

- Managed PostgreSQL istifadə edin (Azure Database, AWS RDS, ...)
- Connection string-də SSL mode aktiv edin
- Backup strategiyası qurun
- Seed-i production-da deaktiv edin (hazırda hər startup-da işləyir)

### Şəkil storage

- `wwwroot/images/products` volume mount lazımdır
- Docker/Kubernetes-də persistent volume istifadə edin
- Gələcək: Cloud storage (S3, Azure Blob)

### Logging

- Console sink yetərli deyil — Seq/Elasticsearch/Azure Monitor əlavə edin
- Log rotation konfiqurasiya edin

---

## Deploy zamanı riskli nöqtələr

| Risk | Təsvir | Həll |
|------|--------|------|
| Auto-migration on startup | Production DB-yə birbaşa migration | CI/CD-də ayrıca migration step |
| Auto-seed on startup | Hər restart-da seed cəhdi | Production-da seed deaktiv et |
| Port mismatch | docker-compose 5434, appsettings 5432 | Connection string uyğunlaşdır |
| Image volume | Container restart-da şəkillər itir | Persistent volume mount |
| No health check | Container orchestrator restart edə bilmir | `/health` endpoint əlavə et |
| No CI/CD | Manual deploy human error riski | GitHub Actions pipeline qur |

---

## CI/CD (tövsiyə — hazırda yoxdur)

Nümunə GitHub Actions workflow:

```yaml
# .github/workflows/deploy.yml (YARADILMAYIB)
name: Deploy ElectroShop
on:
  push:
    branches: [main]
jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build -c Release
      - run: dotnet publish src/ElectroShop.WebApi -c Release -o publish
      # - run: dotnet test (test layihəsi yoxdur)
  deploy:
    needs: build
    steps:
      - run: docker build -f src/ElectroShop.WebApi/Dockerfile -t electroshop-api .
      # Deploy steps...
```

**Status:** `.github/workflows/` qovluğu yoxdur — yalnız `.github/DESCRIPTION.txt` var.

---

## Rollback strategiyası

1. **Application rollback:** Əvvəlki Docker image tag-ına qayıt
2. **Database rollback:** EF Core migration rollback:

```bash
dotnet ef database update PreviousMigrationName \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi
```

3. **Backup:** Deploy-dan əvvəl PostgreSQL backup alın
