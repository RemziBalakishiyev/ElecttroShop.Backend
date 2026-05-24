# Configuration Sənədi

---

## Konfiqurasiya faylları

| Fayl | Mövcud | Təsvir |
|------|--------|--------|
| `src/ElectroShop.WebApi/appsettings.json` | ✅ | Əsas konfiqurasiya |
| `appsettings.Development.json` | ❌ | Yoxdur |
| User Secrets | ✅ | `UserSecretsId` WebApi csproj-də konfiqurasiya edilib |

**Qeyd:** Environment-specific JSON fayl yoxdur. Development/Production fərqləri environment variable-lar və ya user secrets ilə idarə olunmalıdır.

---

## appsettings.json strukturu

### Logging

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

ASP.NET Core default logging — Serilog ilə birgə işləyir.

### ConnectionStrings

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=ElectroShopDb;Username=postgres;Password=postgres"
  }
}
```

**İstifadə:** `Persistence/DependencyInjection.cs` → Npgsql EF Core

**⚠️ Port uyğunsuzluğu:**
- `appsettings.json` → Port **5432**
- `docker-compose.yml` → Port **5434** (host)

Docker PostgreSQL istifadə edərkən connection string-i yeniləyin:

```
Host=localhost;Port=5434;Database=ElectroShopDb;Username=postgres;Password=postgres
```

### ImageStorage

```json
{
  "ImageStorage": {
    "BasePath": "wwwroot/images/products"
  }
}
```

**İstifadə:** `LocalImageStorage` constructor — şəkil fayllarının saxlanma yolu.

Docker/production-da volume mount lazımdır.

### Jwt

```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyForJWTTokenGenerationMustBeAtLeast32CharactersLong!",
    "Issuer": "ElectroShop",
    "Audience": "ElectroShop",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  }
}
```

**İstifadə:**
- `JwtOptions` class (`Application/Common/Options/JwtOptions.cs`)
- JWT Bearer authentication (`WebApplicationBuilderExtensions.cs`)
- `TokenService`

**⚠️ Production:** SecretKey hardcoded-dur — environment variable ilə əvəz edilməlidir.

### Serilog

```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

**İstifadə:** `Program.cs` → `ReadFrom.Configuration()`

### AllowedHosts

```json
{ "AllowedHosts": "*" }
```

---

## Environment dəyişənləri

ASP.NET Core standart override qaydası: Environment variable > appsettings.json

| Dəyişən | Override path | Təsvir |
|---------|---------------|--------|
| `ASPNETCORE_ENVIRONMENT` | — | Development / Production |
| `ConnectionStrings__DefaultConnection` | ConnectionStrings:DefaultConnection | DB connection string |
| `Jwt__SecretKey` | Jwt:SecretKey | JWT secret |
| `Jwt__AccessTokenExpirationMinutes` | Jwt:AccessTokenExpirationMinutes | Token müddəti |
| `ImageStorage__BasePath` | ImageStorage:BasePath | Şəkil yolu |
| `ASPNETCORE_HTTP_PORTS` | — | Docker: 8080 |
| `ASPNETCORE_HTTPS_PORTS` | — | Docker: 8081 |

### Docker environment (launchSettings)

```
ASPNETCORE_HTTPS_PORTS=8081
ASPNETCORE_HTTP_PORTS=8080
```

---

## launchSettings.json

**Fayl:** `src/ElectroShop.WebApi/Properties/launchSettings.json`

| Profile | URL |
|---------|-----|
| http | `http://localhost:5223` |
| https | `https://localhost:7161;http://localhost:5223` |
| IIS Express | `http://localhost:34559` (SSL: 44312) |
| Container (Dockerfile) | Port 8080/8081 |

Swagger launch URL: `/swagger`

---

## DI konfiqurasiya axını

```
Program.cs
  → builder.AddWebApiServices()
      → AddApplication()           // MediatR, FluentValidation, Mapster, cache
      → AddAuthenticationServices() // JwtOptions, TokenService, PasswordHasher
      → AddImageStorage()          // LocalImageStorage, ImageUploadContext
      → AddDiscountServices()      // DiscountCalculationService
      → AddPersistence()           // DbContext, Repositories, UnitOfWork
      → AddControllers()
      → AddCors("AllowAll")
      → AddAuthentication().AddJwtBearer()
      → AddSwaggerGen()
```

---

## Development vs Production

| Aspekt | Development | Production |
|--------|-------------|------------|
| DB | localhost:5432/5434 | Managed PostgreSQL |
| JWT Secret | appsettings.json | Environment variable |
| Swagger | Aktiv | Aktiv (hazırda hər yerdə açıq) |
| CORS | AllowAll | Spesifik origin-lər (tövsiyə) |
| Logging | Console | Console + File/Cloud (tövsiyə) |
| Migration | Startup-da avtomatik | CI/CD pipeline (tövsiyə) |
| Seed | Hər startup-da | Yalnız ilk deploy (tövsiyə) |
| HTTPS | Dev certificate | Reverse proxy TLS |
| Image storage | Lokal wwwroot | Volume mount / Cloud storage |

---

## User Secrets (Development)

WebApi csproj-də `UserSecretsId` konfiqurasiya edilib.

```bash
cd src/ElectroShop.WebApi
dotnet user-secrets set "Jwt:SecretKey" "your-production-secret-key"
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Port=5434;..."
```

---

## External service konfiqurasiyaları

| Xidmət | Konfiqurasiya | Status |
|--------|---------------|--------|
| PostgreSQL | ConnectionStrings:DefaultConnection | Aktiv |
| Lokal fayl storage | ImageStorage:BasePath | Aktiv |
| Email (SMTP/SendGrid) | — | Yoxdur |
| Payment (Stripe/PayPal) | — | Yoxdur |
| Redis cache | — | Yoxdur (IMemoryCache istifadə olunur) |
| Cloud storage (S3/Azure Blob) | — | Yoxdur |
