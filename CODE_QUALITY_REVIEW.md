# Code Quality və Improvement Sənədi

Bu sənəd layihədəki texniki borcları, riskli hissələri və təkmilləşdirmə təkliflərini real kod analizinə əsasən sənədləşdirir.

---

## Texniki borclar

### Yüksək prioritet

| # | Borc | Yer | Təsir |
|---|------|-----|-------|
| 1 | Auth comment edilib | `ProductsController`, `DashboardController` | Yazma endpoint-ləri anonim açıqdır |
| 2 | Role-based authorization yoxdur | Bütün controller-lər | Hər hansı staff token ilə admin əməliyyatları |
| 3 | SHA256 parol hash (salt yoxdur) | `PasswordHasher.cs` | Rainbow table hücumları |
| 4 | JWT Secret hardcoded | `appsettings.json` | Secret source control-da |
| 5 | Test layihəsi yoxdur | Solution | Regression riski |

### Orta prioritet

| # | Borc | Yer | Təsir |
|---|------|-----|-------|
| 6 | CORS AllowAll | `WebApplicationBuilderExtensions.cs` | CSRF/unauthorized access |
| 7 | Auto-seed hər startup-da | `Program.cs` | Production-da lazımsız |
| 8 | Auto-migration hər startup-da | `Program.cs` | Production riski |
| 9 | CreatedBy/UpdatedBy doldurulmur | `ElectroShopDbContext.cs` | Audit trail natamam |
| 10 | Swagger production-da açıq | `WebApplicationExtensions.cs` | API surface exposure |
| 11 | WeatherForecastController scaffold | `Controllers/` | Lazımsız kod |
| 12 | Customer auth yoxdur | Customer entity | Müştəri sifarişi auth olmadan |

### Aşağı prioritet

| # | Borc | Yer | Təsir |
|---|------|-----|-------|
| 13 | Email/payment inteqrasiya yoxdur | — | Manual proseslər |
| 14 | Background job yoxdur | — | Scalable deyil |
| 15 | Cloud image storage yoxdur | `LocalImageStorage` | Horizontal scaling çətin |
| 16 | appsettings.Development.json yoxdur | WebApi | Dev/prod fərqi aydın deyil |

---

## Təkrar kodlar

| Pattern | Harada | Tövsiyə |
|---------|--------|---------|
| Controller action → `command with { Id = id }` | Bütün controller-lər | Minimal — record `with` pattern standartdır |
| Image upload validation | `ProductsController`, `ImagesController` | `FileUploadHelper` artıq ortaq — yaxşıdır |
| Paged query pattern | Hər query handler | Generic pagination helper mövcuddur (`PaginationExtensions`) |
| Error response mapping | `BaseApiController` | Mərkəzləşdirilib — yaxşıdır |
| `[AllowAnonymous]` + `[Authorize]` controller | Brands, Categories | Oxuma/yazma ayrımı — qəbul edilə bilər |

---

## Riskli hissələr

### Təhlükəsizlik riskləri

| Risk | Severity | Detay |
|------|----------|-------|
| Açıq product CRUD | 🔴 Critical | `//[Authorize]` — hər kəs məhsul yarada/silə bilər |
| Açıq dashboard | 🟡 Medium | Satış statistikası anonim əlçatan |
| Açıq image upload | 🟡 Medium | `/api/Images/upload` auth tələb etmir |
| JWT secret in source | 🔴 Critical | Git history-də qalır |
| CORS AllowAll | 🟡 Medium | Cross-origin request-lər |
| SHA256 password | 🔴 Critical | Salt olmadan zəif hash |
| No rate limiting | 🟡 Medium | Brute force login |

### Performance riskləri

| Risk | Yer | Detay |
|------|-----|-------|
| N+1 query potensialı | Product list handler | Include/ThenInclude yoxlanılmalı |
| IMemoryCache limitsiz | Lookup cache | Memory leak potensialı (kiçik data) |
| Startup migration+seed | `Program.cs` | Cold start yavaş |
| Şəkil disk I/O | `LocalImageStorage` | High traffic-da bottleneck |
| Discount hesablama hər məhsulda | `DiscountCalculationService` | 3 DB query/məhsul — batch lazım ola bilər |

### Data integrity riskləri

| Risk | Detay |
|------|-------|
| Concurrency | RowVersion yalnız Product/Order-da |
| Soft delete vs hard delete | `WriteRepository.Delete()` hard delete edir |
| Order status limited | Yalnız Pending→Paid implement edilib |
| Customer unique phone | Null filter ilə — OK |

---

## Refactor təklifləri

### 1. Authorization middleware/policy

```csharp
// Tövsiyə: Program.cs və ya Extensions
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Admin", "Agent"));
});
```

### 2. PasswordHasher → BCrypt

```csharp
// PasswordHasher.cs əvəzinə BCrypt.Net-Next
public string HashPassword(string password) => BCrypt.HashPassword(password);
public bool VerifyPassword(string password, string hash) => BCrypt.Verify(password, hash);
```

### 3. ICurrentUserService

```csharp
// CreatedBy/UpdatedBy avtomatik doldurulması
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
}
```

### 4. Test layihəsi

```
tests/ElectroShop.Application.Tests/
tests/ElectroShop.Domain.Tests/
tests/ElectroShop.WebApi.IntegrationTests/
```

---

## Performance təkmilləşdirmələri

| Təklif | Prioritet | Təsir |
|--------|-----------|-------|
| Discount batch loading (list endpoint) | Yüksək | List query sürəti |
| Redis cache (lookup + discount) | Orta | Scalability |
| CDN/cloud storage for images | Orta | Static content delivery |
| Database index review | Orta | Query performance |
| Response compression | Aşağı | Bandwidth |
| Pagination default limit | Aşağı | Memory usage |

---

## Security təkmilləşdirmələri

| Təklif | Prioritet |
|--------|-----------|
| `[Authorize]` aktivləşdir (Products, Dashboard) | 🔴 Critical |
| Role-based policies | 🔴 Critical |
| JWT secret → environment variable | 🔴 Critical |
| BCrypt password hashing | 🔴 Critical |
| CORS spesifik origin-lər | 🟡 Medium |
| Rate limiting (login) | 🟡 Medium |
| Swagger production disable | 🟡 Medium |
| Input sanitization (XSS) | 🟡 Medium |
| HTTPS strict | 🟡 Medium |
| Security headers middleware | Aşağı |

---

## Kod keyfiyyəti — müsbət cəhətlər

| Aspekt | Qiymət |
|--------|--------|
| Clean Architecture ayrımı | ✅ Yaxşı |
| CQRS + MediatR | ✅ Yaxşı |
| Result Pattern | ✅ Yaxşı |
| FluentValidation pipeline | ✅ Yaxşı |
| Domain-driven entity design | ✅ Yaxşı |
| Global exception handling | ✅ Yaxşı |
| Soft delete pattern | ✅ Yaxşı |
| Optimistic concurrency | ✅ Yaxşı (Product/Order) |
| Structured logging (Serilog) | ✅ Yaxşı |
| Swagger documentation | ✅ Yaxşı |
| Mapster mapping | ✅ Yaxşı |
| Aggregate root pattern | ✅ Yaxşı |

---

## Prioritetli action plan

### Sprint 1 (Critical)
1. ProductsController və DashboardController-da auth aktivləşdir
2. JWT secret environment variable-a köçür
3. PasswordHasher → BCrypt

### Sprint 2 (Important)
4. Role-based authorization policies
5. Unit test layihəsi yarat (domain + validators)
6. Production-da seed/migration davranışını ayır

### Sprint 3 (Enhancement)
7. CI/CD pipeline (GitHub Actions)
8. Integration testlər
9. Health check endpoint
10. Email notification (ProductPriceChanged)
