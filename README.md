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

# 2. Environment dəyişənlərini təyin et (.env.example-dan kopyala)
cp .env.example .env
# ConnectionStrings__DefaultConnection, JWT__Key və s. doldurun

# 3. Layihəni işə sal
dotnet run --project src/ElectroShop.WebApi
```

- **Swagger:** `http://localhost:5223/swagger`
- **Default admin:** `admin@electroshop.az` / `Admin123!`

Ətraflı setup: [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md)

---

## Render-də Production Deploy

### 1. PostgreSQL (Render Database)

1. [Render Dashboard](https://dashboard.render.com) → **New** → **PostgreSQL**
2. Name, region və plan seçin
3. Database yaradıldıqdan sonra **Internal Database URL** və ya **External Database URL** kopyalayın
4. Web Service ilə eyni region-da saxlayın (internal URL daha sürətlidir)

### 2. Backend Web Service (Docker)

1. **New** → **Web Service** → GitHub repo-nu bağlayın
2. Parametrlər:

| Parametr | Dəyər |
|----------|-------|
| **Environment** | Docker |
| **Dockerfile Path** | `Dockerfile` (repo root) |
| **Docker Build Context** | `.` (repo root) |
| **Health Check Path** | `/health` |

3. **Environment Variables** (Render dashboard-da):

| Variable | Dəyər |
|----------|-------|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ASPNETCORE_URLS` | `http://0.0.0.0:10000` |
| `ConnectionStrings__DefaultConnection` | Render PostgreSQL connection string |
| `MIGRATE_ON_STARTUP` | `true` *(yalnız ilk deploy üçün; sonra `false` edin)* |
| `FRONTEND_URL` | Static Site URL (məs: `https://electroshop.onrender.com`) |
| `JWT__Key` | Min 32 simvol təsadüfi secret |
| `JWT__Issuer` | `ElectroShop` |
| `JWT__Audience` | `ElectroShop` |

4. Deploy tamamlandıqdan sonra `https://<api-service>.onrender.com/health` → `OK` qaytarmalıdır

### 3. Frontend Static Site (React / Vite)

Frontend ayrı repo və ya monorepo qovluğundadırsa:

1. **New** → **Static Site** → eyni GitHub repo (və ya frontend repo)
2. **Build Command:** `npm install && npm run build`
3. **Publish Directory:** `dist`
4. **Environment Variables:**

| Variable | Dəyər |
|----------|-------|
| `VITE_API_BASE_URL` | Backend Web Service URL (məs: `https://electroshop-api.onrender.com`) |

5. **Redirects/Rewrites** (React Router üçün):

| Source | Destination |
|--------|-------------|
| `/*` | `/index.html` |

6. Frontend API client-lər `import.meta.env.VITE_API_BASE_URL` istifadə etməlidir — hardcoded URL olmamalıdır

### 4. Lokal development

```bash
# .env.example → .env kopyalayın və dəyərləri doldurun
cp .env.example .env

# PostgreSQL
docker-compose up -d

# Backend (env dəyişənləri ilə)
dotnet run --project src/ElectroShop.WebApi
```

Lokal JWT və DB üçün `dotnet user-secrets` və ya `.env` + `ConnectionStrings__DefaultConnection` istifadə edin.

### 5. Docker build (lokal yoxlama)

```bash
docker build -t electroshop-backend .
docker run -p 10000:10000 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  -e ASPNETCORE_URLS=http://0.0.0.0:10000 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5434;Database=ElectroShopDb;Username=postgres;Password=YOUR_PASSWORD" \
  -e JWT__Key="your-local-jwt-secret-at-least-32-chars" \
  -e JWT__Issuer=ElectroShop \
  -e JWT__Audience=ElectroShop \
  -e MIGRATE_ON_STARTUP=true \
  -e FRONTEND_URL=http://localhost:5173 \
  electroshop-backend
```

Ətraflı: [DEPLOYMENT.md](./DEPLOYMENT.md)

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
