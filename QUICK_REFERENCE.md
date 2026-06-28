# Developer Quick Reference

Tez istinad sənədi — ən çox istifadə olunan əmrlər, folder-lər, class-lar və endpoint-lər.

---

## Tez-tez istifadə olunan əmrlər

```bash
# Layihəni işə sal
dotnet run --project src/ElectroShop.WebApi

# Build
dotnet build

# Restore
dotnet restore

# Migration yarat
dotnet ef migrations add MigrationName \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi

# Database yenilə
dotnet ef database update \
  --project src/ElectroShop.Persistence \
  --startup-project src/ElectroShop.WebApi

# Docker PostgreSQL
docker-compose up -d
docker-compose down
docker-compose logs -f postgres

# Docker API build
docker build -t electroshop-backend .

# Publish
dotnet publish src/ElectroShop.WebApi -c Release -o ./publish

# Dev HTTPS sertifikat
dotnet dev-certs https --trust
```

---

## Vacib folder-lər

| Path | Məzmun |
|------|--------|
| `src/ElectroShop.Domain/Entities/` | Domain entity-lər |
| `src/ElectroShop.Domain/ValueObjects/` | Money, Sku |
| `src/ElectroShop.Application/Features/` | CQRS commands/queries |
| `src/ElectroShop.Application/DTOs/` | API modelləri |
| `src/ElectroShop.Application/Services/` | Application service-lər |
| `src/ElectroShop.Application/Common/Results/` | Result pattern |
| `src/ElectroShop.Persistence/Contexts/` | DbContext |
| `src/ElectroShop.Persistence/Configurations/` | EF Core configs |
| `src/ElectroShop.Persistence/Repositories/` | Repository impl |
| `src/ElectroShop.Persistence/Migrations/` | DB migration-lar |
| `src/ElectroShop.WebApi/Controllers/` | API controller-lər |
| `src/ElectroShop.WebApi/Middleware/` | Custom middleware |
| `scripts/` | Migration skriptləri |

---

## Vacib class-lar

### Domain

| Class | Fayl | Rol |
|-------|------|-----|
| `Product` | `Domain/Entities/Product.cs` | Məhsul aggregate root |
| `Order` | `Domain/Entities/Order.cs` | Sifariş aggregate root |
| `Money` | `Domain/ValueObjects/Money.cs` | Pul value object |
| `Sku` | `Domain/ValueObjects/Sku.cs` | SKU value object |
| `AggregateRoot` | `Domain/Primitives/AggregateRoot.cs` | RowVersion base |
| `BaseCommonEntity` | `Domain/Primitives/BaseCommonEntity.cs` | Audit + soft delete |

### Application

| Class | Fayl | Rol |
|-------|------|-----|
| `Result<T>` | `Application/Common/Results/Result.cs` | Result pattern |
| `DomainErrors` | `Application/Common/Results/DomainErrors.cs` | Xəta kodları |
| `TokenService` | `Application/Services/TokenService.cs` | JWT generasiya |
| `PasswordHasher` | `Application/Services/PasswordHasher.cs` | Parol hash |
| `LocalImageStorage` | `Application/Services/LocalImageStorage.cs` | Şəkil storage |
| `DiscountCalculationService` | `Application/Services/DiscountCalculationService.cs` | Endirim hesab |
| `LoggingBehaviour` | `Application/Behaviours/LoggingBehaviour.cs` | Request logging |
| `ValidationBehaviour` | `Application/Behaviours/ValidationBehaviour.cs` | FluentValidation |

### Persistence

| Class | Fayl | Rol |
|-------|------|-----|
| `ElectroShopDbContext` | `Persistence/Contexts/ElectroShopDbContext.cs` | EF Core context |
| `UnitOfWork` | `Persistence/Repositories/UnitOfWork.cs` | Transaction + events |
| `DatabaseSeeder` | `Persistence/Seeders/DatabaseSeeder.cs` | Seed data |

### WebApi

| Class | Fayl | Rol |
|-------|------|-----|
| `BaseApiController` | `WebApi/Controllers/BaseApiController.cs` | Result → HTTP mapping |
| `ExceptionHandlingMiddleware` | `WebApi/Middleware/ExceptionHandlingMiddleware.cs` | Global exception |
| `WebApplicationBuilderExtensions` | `WebApi/Extensions/WebApplicationBuilderExtensions.cs` | DI setup |

---

## Vacib endpoint-lər

### Auth
```
POST /api/Auth/login
POST /api/Auth/refresh-token
```

### Məhsullar
```
GET  /api/Products                    # Siyahı (filtr, səhifələmə)
GET  /api/Products/{id}               # Detal
GET  /api/Products/search             # Axtarış
GET  /api/Products/banner             # Banner
GET  /api/Products/featured           # Featured (5)
POST /api/Products                    # Yarat
PUT  /api/Products/{id}               # Yenilə
```

### Kateqoriya / Brend
```
GET  /api/Categories/lookup           # Dropdown
GET  /api/Brands/lookup               # Dropdown
GET  /api/Brands/promotional          # Promotional brendlər
```

### Sifariş
```
POST /api/Orders                      # Sifariş yarat
POST /api/Orders/{id}/items           # Məhsul əlavə
PATCH /api/Orders/{id}/mark-paid        # Ödəniş qeydi
```

### Endirim
```
GET  /api/discounts                   # Siyahı
POST /api/discounts                   # Yarat
```

### Dashboard
```
GET  /api/dashboard                   # Statistika
GET  /api/dashboard/chart             # Qrafik
```

### Şəkil
```
POST /api/Images/upload               # Yüklə → imageId
GET  /api/Images/{imageId}.jpg        # Oxu
```

---

## Default credentials

| Rol | Email | Parol |
|-----|-------|-------|
| Admin | admin@electroshop.az | Admin123! |
| Agent | agent1@electroshop.az | Agent123! |
| Agent | agent2@electroshop.az | Agent123! |

---

## Debug zamanı baxılmalı yerlər

### Auth problemi
1. `Features/Auth/Commands/Login/LoginCommandHandler.cs`
2. `Services/TokenService.cs`
3. `appsettings.json` → Jwt section
4. Swagger → Authorize düyməsi

### Validasiya xətası
1. `Features/{Modul}/Commands/{Command}/*Validator.cs`
2. `Behaviours/ValidationBehaviour.cs`
3. `BaseApiController.HandleResult()` — ValidationResult handling

### DB xətası
1. `Persistence/Contexts/ElectroShopDbContext.cs`
2. Connection string (`appsettings.json`)
3. Pending migration-lar (`Migrations/` folder)
4. `Program.cs` — migration log-ları

### Concurrency xətası (409)
1. `Persistence/Repositories/UnitOfWork.cs` — ConcurrencyException
2. `Middleware/ExceptionHandlingMiddleware.cs` — 409 mapping
3. Entity `RowVersion` / `xmin`

### Şəkil problemi
1. `Services/LocalImageStorage.cs`
2. `wwwroot/images/products/` qovluğu
3. `Controllers/ImagesController.cs`
4. Max 10 MB, `.jpg/.png/.webp/.gif`

### Endirim hesablanmır
1. `Services/DiscountCalculationService.cs`
2. Prioritet: Product > Brand > Category
3. `Discount.IsActive`, `StartDate`, `EndDate` yoxla

### Məhsul tapılmır (404)
1. `IsDeleted = true` olub olmadığı (soft delete)
2. `IsActive = false` olub olmadığı
3. Global query filter: `BaseCommonEntityConfiguration`

---

## URL-lər (Development)

| Xidmət | URL |
|--------|-----|
| API (HTTP) | http://localhost:5223 |
| API (HTTPS) | https://localhost:7161 |
| Swagger | http://localhost:5223/swagger |
| PostgreSQL (Docker) | localhost:5434 |

---

## Sənəd linkləri

| Sənəd | Məzmun |
|-------|--------|
| [README.md](./README.md) | Ümumi baxış |
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Arxitektura |
| [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) | Endpoint-lər |
| [DATABASE.md](./DATABASE.md) | DB schema |
| [BUSINESS_RULES.md](./BUSINESS_RULES.md) | Biznes qaydaları |
| [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) | Setup guide |
