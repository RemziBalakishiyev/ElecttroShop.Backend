# Development Guide

Yeni developer üçün addım-addım lokal setup təlimatı.

---

## Prerequisites

| Alət | Versiya | Link |
|------|---------|------|
| .NET SDK | 8.0+ | https://dotnet.microsoft.com/download/dotnet/8.0 |
| PostgreSQL | 16+ | https://www.postgresql.org/download/ |
| Docker Desktop | (optional) | https://www.docker.com/products/docker-desktop |
| IDE | VS 2022 / VS Code / Rider | — |
| EF Core CLI | global tool | `dotnet tool install --global dotnet-ef` |

---

## 1. Repository clone

```bash
git clone <repository-url>
cd ElectronicNumberOne
```

---

## 2. Dependency restore

```bash
dotnet restore
```

**Solution:** `ElectronicNumberOne.sln`

**Layihələr:**
- `src/ElectroShop.Domain`
- `src/ElectroShop.Application`
- `src/ElectroShop.Persistence`
- `src/ElectroShop.WebApi`

---

## 3. Database setup

### Variant A: Docker (tövsiyə olunur)

```bash
docker-compose up -d
```

PostgreSQL `localhost:5434`-də işləyir (container daxilində 5432).

**Connection string-i yeniləyin** (`src/ElectroShop.WebApi/appsettings.json`):

```
Host=localhost;Port=5434;Database=ElectroShopDb;Username=postgres;Password=postgres
```

### Variant B: Lokal PostgreSQL

1. PostgreSQL quraşdırın
2. `ElectroShopDb` database yaradın
3. Connection string-i `appsettings.json`-da tənzimləyin (default port 5432)

---

## 4. Migration

### Avtomatik (tövsiyə)

Layihə startup-da pending migration-ları avtomatik tətbiq edir (`Program.cs`).

Sadəcə `dotnet run` kifayətdir.

### Manual

```bash
# Migration yaratmaq
dotnet ef migrations add MigrationName \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi

# Database yeniləmək
dotnet ef database update \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi
```

### PowerShell skriptləri

```powershell
# Migration yarat
.\scripts\create-migration.ps1 -MigrationName "AddNewFeature"

# Database yenilə
.\scripts\update-database.ps1
```

---

## 5. Layihəni işə salmaq

```bash
dotnet run --project src/ElectroShop.WebApi
```

**URL-lər:**

| Profile | URL |
|---------|-----|
| HTTP | http://localhost:5223 |
| HTTPS | https://localhost:7161 |
| Swagger | http://localhost:5223/swagger |

---

## 6. İlk test

### Swagger UI

1. Brauzerdə `/swagger` açın
2. `POST /api/Auth/login` — body:

```json
{
  "email": "admin@electroshop.az",
  "password": "Admin123!"
}
```

3. Qayıdan `accessToken`-i Swagger "Authorize" düyməsinə daxil edin
4. `GET /api/Products` — məhsul siyahısını yoxlayın

---

## Build əmrləri

```bash
# Build (bütün solution)
dotnet build

# Release build
dotnet build -c Release

# Tək layihə
dotnet build src/ElectroShop.WebApi
```

---

## Test əmrləri

**⚠️ Test layihəsi mövcud deyil.** Solution-da unit/integration test project yoxdur.

```bash
dotnet test   # Hal-hazırda işlədilə bilmir
```

---

## Yeni feature əlavə etmə

### CQRS feature yaratma

```
src/ElectroShop.Application/Features/{Modul}/
├── Commands/{CommandName}/
│   ├── {CommandName}Command.cs
│   ├── {CommandName}CommandHandler.cs
│   └── {CommandName}CommandValidator.cs
└── Queries/{QueryName}/
    ├── {QueryName}Query.cs
    └── {QueryName}QueryHandler.cs
```

MediatR handler-lər avtomatik qeydiyyat olunur (`DependencyInjection.cs`).

### Controller endpoint əlavə etmə

1. Controller-da action yaradın
2. `Mediator.Send(command/query)` çağırın
3. `HandleResult()` / `HandlePagedResult()` ilə response qaytarın

### Migration yaratma

Entity dəyişikliyindən sonra:

```bash
dotnet ef migrations add DescriptiveName \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi
```

---

## Faydalı fayllar

| Fayl | Məqsəd |
|------|--------|
| `PMC-Commands.txt` | Visual Studio Package Manager Console əmrləri |
| `scripts/create-migration.ps1` | Migration yaratma skripti |
| `scripts/update-database.ps1` | Database yeniləmə skripti |
| `scripts/apply-pending-migrations.sql` | Manual SQL migration |
| `REPOSITORY_USAGE.md` | Repository pattern istifadəsi |

---

## Tez-tez rast gəlinən problemlər

### Port conflict (5432)

Docker compose 5434 istifadə edir. Connection string portunu yoxlayın.

### Migration xətası

```bash
dotnet ef database update --project src/ElectroShop.Persistence --startup-project src/ElectroShop.WebApi
```

### HTTPS sertifikat xətası (Development)

```bash
dotnet dev-certs https --trust
```

---

## IDE konfiqurasiyası

### Visual Studio

- Startup project: `ElectroShop.WebApi`
- Launch profile: `https` və ya `http`
- Package Manager Console: `PMC-Commands.txt`-dəki əmrlər

### VS Code

```json
// .vscode/launch.json (yoxdursa yaradın)
{
  "configurations": [
    {
      "name": "ElectroShop WebApi",
      "type": "coreclr",
      "request": "launch",
      "program": "${workspaceFolder}/src/ElectroShop.WebApi/bin/Debug/net8.0/ElectroShop.WebApi.dll",
      "cwd": "${workspaceFolder}/src/ElectroShop.WebApi",
      "launchBrowser": { "url": "http://localhost:5223/swagger" }
    }
  ]
}
```

**Qeyd:** `.vscode/launch.json` hazırda repo-da yoxdur — dəqiqləşdirilməlidir.
