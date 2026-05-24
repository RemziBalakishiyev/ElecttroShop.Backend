# ElectroShop API

**ElectroShop** — elektronika və məişət texnikası satışı üçün hazırlanmış, ASP.NET Core 8.0 əsaslı e-ticarət backend API-sidir.

| Texnologiya | Versiya |
|-------------|---------|
| .NET | 8.0 |
| PostgreSQL | 16 |
| EF Core | 8.0 |
| Arxitektura | Clean Architecture + DDD + CQRS |

---

## Layihə nə üçündür?

ElectroShop, onlayn mağaza (frontend) və admin paneli üçün backend xidmətləri təmin edir:

- Məhsul kataloqu (axtarış, filtr, variantlar, şəkillər)
- Kateqoriya və brend idarəetməsi
- Endirim sistemi
- Sifariş idarəetməsi
- Admin dashboard statistikası
- JWT əsaslı staff autentifikasiyası
- Müştəri qeydiyyatı

## Hansı problemi həll edir?

Tək monolit backend olaraq e-ticarət biznes məntiqini (məhsul, stok, endirim, sifariş) strukturlaşdırılmış, genişlənə bilən API ilə təqdim edir. Frontend və admin panel bu API-yə qoşularaq işləyir.

## Əsas modullar

| Modul | Təsvir |
|-------|--------|
| **Auth** | Staff login, refresh token |
| **Products** | Məhsul CRUD, stok/qiymət, banner/featured, variantlar, şəkillər |
| **Categories** | Kateqoriya ağacı, atributlar və dəyərlər |
| **Brands** | Brend idarəetməsi, promotional brendlər |
| **Discounts** | Məhsul/brend/kateqoriya endirimləri |
| **Orders** | Sifariş yaratma, element əlavə/silmə, ödəniş qeydi |
| **Customers** | Müştəri qeydiyyatı və profil |
| **Dashboard** | Statistika və qrafik məlumatları |
| **Images** | Şəkil yükləmə və oxuma |

## Sistem kimlər üçün nəzərdə tutulub?

| İstifadəçi | Rol | İstifadə |
|------------|-----|----------|
| **Admin** | `UserRole.Admin` | Tam idarəetmə (brend, kateqoriya, endirim, sifariş) |
| **Agent** | `UserRole.Agent` | Staff əməliyyatları |
| **Müştəri (Customer)** | Ayrı entity | Qeydiyyat, sifariş *(JWT yoxdur)* |
| **Frontend istifadəçisi** | Anonim | Məhsul/kateqoriya/brend oxuma |

---

## Solution strukturu

```
ElectronicNumberOne/
├── src/
│   ├── ElectroShop.Domain/         # Entity, Value Object, Enum, Domain Event
│   ├── ElectroShop.Application/    # CQRS, DTO, Validator, Service
│   ├── ElectroShop.Persistence/    # EF Core, Repository, Migration, Seeder
│   └── ElectroShop.WebApi/         # Controller, Middleware, Swagger
├── scripts/                        # Migration skriptləri
├── docker-compose.yml              # PostgreSQL konteyneri
└── docs/                           # Sənədləşdirmə (aşağıdakı fayllar)
```

---

## Sənədləşdirmə

| Sənəd | Məzmun |
|-------|--------|
| [ARCHITECTURE.md](./ARCHITECTURE.md) | Arxitektura, layer-lər, design pattern-lər |
| [API_DOCUMENTATION.md](./API_DOCUMENTATION.md) | Bütün endpoint-lər, request/response |
| [DATABASE.md](./DATABASE.md) | Entity-lər, cədvəllər, əlaqələr |
| [BUSINESS_RULES.md](./BUSINESS_RULES.md) | Biznes qaydaları və user flow-lar |
| [AUTHENTICATION_AUTHORIZATION.md](./AUTHENTICATION_AUTHORIZATION.md) | JWT, rollar, access control |
| [CONFIGURATION.md](./CONFIGURATION.md) | appsettings, environment dəyişənləri |
| [LOGGING_ERROR_HANDLING.md](./LOGGING_ERROR_HANDLING.md) | Serilog, exception handling |
| [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md) | Lokal setup, migration, build |
| [DEPLOYMENT.md](./DEPLOYMENT.md) | Docker, production qeydləri |
| [CODE_QUALITY_REVIEW.md](./CODE_QUALITY_REVIEW.md) | Texniki borc, risklər, təkliflər |
| [QUICK_REFERENCE.md](./QUICK_REFERENCE.md) | Tez istinad: əmrlər, class-lar, endpoint-lər |

---

## Tez başlanğıc

```bash
# 1. PostgreSQL (Docker)
docker-compose up -d

# 2. Connection string-i yoxla (Port 5434 üçün docker-compose)
# src/ElectroShop.WebApi/appsettings.json

# 3. Layihəni işə sal
dotnet run --project src/ElectroShop.WebApi
```

- **Swagger:** `http://localhost:5223/swagger`
- **Default admin:** `admin@electroshop.az` / `Admin123!`

Ətraflı setup: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)

---

## Seed məlumatları

İlk işə salınmada avtomatik:

- 3 staff istifadəçi (1 Admin, 2 Agent)
- 7 kateqoriya (iyerarxik)
- 10 brend (Apple, Samsung, Lenovo, ...)
- 5 nümunə məhsul

---

## Mövcud məhdudiyyətlər

- **Test layihəsi yoxdur** — `dotnet test` işlədilə bilmir
- **CI/CD pipeline yoxdur**
- **ProductsController və DashboardController** — `[Authorize]` comment edilib (açıq endpoint-lər)
- **Role-based authorization policy** tətbiq edilməyib

---

## Lisenziya

MIT License — [LICENSE](./LICENSE) faylına baxın.
