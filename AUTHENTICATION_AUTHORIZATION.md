# Authentication və Authorization Sənədi

---

## Ümumi baxış

ElectroShop iki ayrı istifadəçi tipinə malikdir:

| Tip | Entity | Auth mexanizmi |
|-----|--------|----------------|
| **Staff** (Admin, Agent, ...) | `User` | JWT access + refresh token |
| **Müştəri** | `Customer` | Auth yoxdur (yalnız qeydiyyat) |

---

## Login / Register

### Staff Login

**Endpoint:** `POST /api/Auth/login`

**Flow:**
```
1. Email + Password göndərilir
2. LoginCommandValidator — email format, parol min 6 simvol
3. LoginCommandHandler:
   a. Email lowercase normalizasiya
   b. User tapılır (IsDeleted = false)
   c. IsActive yoxlanılır
   d. PasswordHasher.VerifyPassword()
   e. Access token + Refresh token generasiya
   f. RefreshToken DB-yə yazılır
4. LoginResponseDto qaytarılır
```

**Fayllar:**
- `Features/Auth/Commands/Login/LoginCommandHandler.cs`
- `Services/TokenService.cs`
- `Services/PasswordHasher.cs`

### Refresh Token

**Endpoint:** `POST /api/Auth/refresh-token`

**Flow:**
```
1. RefreshToken DB-dən tapılır
2. IsUsed, IsRevoked, ExpiresAt yoxlanılır
3. Köhnə token IsUsed = true
4. Yeni access + refresh token cütü generasiya
5. Yeni RefreshToken DB-yə yazılır
```

### Müştəri Qeydiyyatı

**Endpoint:** `POST /api/Customers/register` — `[AllowAnonymous]`

- JWT verilmir
- Parol saxlanmır
- Yalnız profil məlumatları (FullName, Email, Phone)

---

## JWT Token

### Konfiqurasiya

`appsettings.json` → `Jwt` section:

| Parametr | Default | Təsvir |
|----------|---------|--------|
| SecretKey | (hardcoded) | HMAC-SHA256 açarı (min 32 simvol) |
| Issuer | ElectroShop | Token issuer |
| Audience | ElectroShop | Token audience |
| AccessTokenExpirationMinutes | 60 | Access token müddəti |
| RefreshTokenExpirationDays | 30 | Refresh token müddəti |

**Options class:** `Application/Common/Options/JwtOptions.cs`

### Access Token Claims

| Claim | Dəyər |
|-------|-------|
| `ClaimTypes.NameIdentifier` | User.Id |
| `ClaimTypes.Email` | User.Email |
| `ClaimTypes.Name` | User.FullName |
| `ClaimTypes.Role` | UserRole enum string |
| `role` (custom) | UserRole enum string |

### Token generasiya

```csharp
// TokenService.GenerateAccessToken()
Algorithm: HMAC-SHA256
ClockSkew: TimeSpan.Zero
```

### Swagger-da istifadə

```
Authorization: Bearer {accessToken}
```

Swagger UI-da "Authorize" düyməsi mövcuddur.

---

## Role sistemi

### UserRole enum

| Rol | Dəyər | Təsvir |
|-----|-------|--------|
| Admin | 1 | Tam idarəetmə |
| Agent | 2 | Staff əməliyyatları |

**Fayl:** `Domain/Enums/UserRole.cs`

### Seed istifadəçilər

| Email | Parol | Rol |
|-------|-------|-----|
| admin@electroshop.az | Admin123! | Admin |
| agent1@electroshop.az | Agent123! | Agent |
| agent2@electroshop.az | Agent123! | Agent |

---

## Authorization

### Controller səviyyəsində

| Controller | Attribute | Qeyd |
|------------|-----------|------|
| AuthController | `[AllowAnonymous]` | Hamısı açıq |
| BrandsController | `[Authorize]` | Oxuma: `[AllowAnonymous]` |
| CategoriesController | `[Authorize]` | Oxuma: `[AllowAnonymous]` |
| CustomersController | `[Authorize]` | Register: `[AllowAnonymous]` |
| OrdersController | `[Authorize]` | Hamısı auth tələb edir |
| DiscountsController | `[Authorize]` | Hamısı auth tələb edir |
| ProductsController | `//[Authorize]` | **Comment edilib — açıq** |
| DashboardController | `//[Authorize]` | **Comment edilib — açıq** |
| ImagesController | Attribute yoxdur | Açıq |

### Role-based policy

**⚠️ Tətbiq edilməyib.** Heç bir `[Authorize(Roles = "...")]` və ya `AddPolicy()` yoxdur.

JWT-də role claim verilir, amma endpoint səviyyəsində role yoxlanılmır. Hər `[Authorize]` endpoint-i hər hansı valid token ilə açılır.

**Tövsiyə:** Admin-only endpoint-lər üçün role policy əlavə edilməlidir.

---

## Parol hash

**Implementasiya:** `PasswordHasher.cs`

| Metod | Alqoritm |
|-------|----------|
| HashPassword | SHA256 |
| VerifyPassword | SHA256 compare |

**⚠️ Təhlükəsizlik riski:** Salt istifadə edilmir. Production-da bcrypt/Argon2 tövsiyə olunur.

---

## RefreshToken entity

| Field | Məna |
|-------|------|
| UserId | FK → Users |
| Token | Base64 random string |
| ExpiresAt | Bitmə tarixi |
| IsUsed | Token istifadə edilib |
| IsRevoked | Token ləğv edilib |
| RevokedAt | Ləğv tarixi |

**Domain qaydaları:**
- Artıq istifadə edilmiş token yenidən istifadə edilə bilməz
- Ləğv edilmiş token qəbul edilmir
- Expired token qəbul edilmir

---

## Security middleware-lər

### Pipeline sırası

```
ExceptionHandling → Swagger → HTTPS → CORS → Authentication → Authorization
```

### CORS

Policy: `"AllowAll"` — hər hansı origin, method, header

**⚠️ Production riski:** Spesifik origin-lər ilə məhdudlaşdırılmalıdır.

### HTTPS

`UseHttpsRedirection()` aktivdir.

---

## Access control xülasəsi

| Endpoint qrupu | Anonim | Auth tələb | Role tələb |
|----------------|--------|------------|------------|
| Auth login/refresh | ✅ | ❌ | ❌ |
| Products oxuma | ✅ | ❌ | ❌ |
| Products yazma | ✅ ⚠️ | ❌ | ❌ |
| Categories/Brands oxuma | ✅ | ❌ | ❌ |
| Categories/Brands yazma | ❌ | ✅ | ❌ |
| Orders | ❌ | ✅ | ❌ |
| Discounts | ❌ | ✅ | ❌ |
| Dashboard | ✅ ⚠️ | ❌ | ❌ |
| Customer register | ✅ | ❌ | ❌ |
| Customer oxuma/yeniləmə | ❌ | ✅ | ❌ |
| Images | ✅ | ❌ | ❌ |

**⚠️** = Hazırda təhlükəsizlik riski — auth comment edilib və ya attribute yoxdur

---

## Production tövsiyələri

1. `ProductsController` və `DashboardController`-da `[Authorize]` aktivləşdir
2. Role-based policy əlavə et (Admin-only CRUD)
3. JWT SecretKey-i environment variable-dan oxu
4. PasswordHasher-i bcrypt/Argon2 ilə əvəz et
5. CORS-u spesifik origin-lərlə məhdudlaşdır
6. Rate limiting əlavə et (login endpoint)
