# Logging və Error Handling Sənədi

---

## Logging sistemi

### Serilog (Host səviyyəsi)

**Konfiqurasiya:** `Program.cs`

```csharp
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();
```

**Sink:** Yalnız Console (file/cloud sink yoxdur)

**Log səviyyələri:**

| Source | Level |
|--------|-------|
| Default | Information |
| Microsoft | Warning |
| System | Warning |

**Output format:**
```
[HH:mm:ss INF] Message
```

### Startup log-ları

`Program.cs` aşağıdakıları log edir:

| Event | Level |
|-------|-------|
| Application start | Information |
| Pending migrations count | Information |
| Migration applied | Information |
| Seeding completed | Information |
| Startup failure | Fatal |
| Migration/seed error | Error |

### Application səviyyəsi log-lar

| Komponent | Fayl | Nə log edir |
|-----------|------|-------------|
| `LoggingBehaviour<T,T>` | `Application/Behaviours/LoggingBehaviour.cs` | Hər MediatR request adı + elapsed ms |
| `ExceptionHandlingMiddleware` | `WebApi/Middleware/ExceptionHandlingMiddleware.cs` | Unhandled exception-lar |
| `LocalImageStorage` | `Application/Services/LocalImageStorage.cs` | Upload/delete/read əməliyyatları |
| `ProductPriceChangedHandler` | `Application/EventHandlers/ProductPriceChangedHandler.cs` | Qiymət dəyişikliyi event-ləri |
| `ProductsController` | `WebApi/Controllers/ProductsController.cs` | Şəkil upload validasiya xətaları |
| `ImagesController` | `WebApi/Controllers/ImagesController.cs` | Tapılmayan şəkillər |

### MediatR Pipeline Logging

```
Request → LoggingBehaviour (start timer)
       → ValidationBehaviour
       → Handler
       → LoggingBehaviour (log elapsed ms, error on failure)
```

**Nümunə log:**
```
Handling CreateProductCommand
Handled CreateProductCommand in 245ms
```

---

## Error Handling

### 3 səviyyəli xəta idarəetməsi

```
1. FluentValidation → ValidationBehaviour → ValidationResult → 400 Bad Request
2. Handler → Result.Failure(Error) → BaseApiController → HTTP status
3. Unhandled Exception → ExceptionHandlingMiddleware → JSON error
```

### Result Pattern

**Fayl:** `Application/Common/Results/Result.cs`, `Error.cs`

```csharp
Result<T>.Success(value)     // Uğur
Result<T>.Failure(error)     // Xəta
ValidationResult<T>          // Çoxlu validasiya xətaları
```

**ErrorType → HTTP Status mapping** (`BaseApiController.HandleFailure`):

| ErrorType | HTTP Status |
|-----------|-------------|
| Validation (2) | 400 Bad Request |
| NotFound (3) | 404 Not Found |
| Unauthorized (5) | 401 Unauthorized |
| Forbidden (6) | 403 Forbidden |
| Conflict (4) | 409 Conflict |
| Failure (1) | 500 Internal Server Error |

### ExceptionHandlingMiddleware

**Fayl:** `WebApi/Middleware/ExceptionHandlingMiddleware.cs`

Pipeline-da **birinci** middleware kimi qeydiyyat edilib.

| Exception | HTTP Status | Error Code |
|-----------|-------------|------------|
| `ArgumentException` | 400 | Validation.ArgumentError |
| `InvalidOperationException` | 400 | Validation.InvalidOperation |
| `UnauthorizedAccessException` | 401 | Authentication.Unauthorized |
| `ConcurrencyException` | 409 | Entity.ConcurrencyConflict |
| `DbUpdateConcurrencyException` | 409 | Entity.ConcurrencyConflict |
| Digər | 500 | General.ServerError |

### DomainErrors kataloqu

**Fayl:** `Application/Common/Results/DomainErrors.cs`

Strukturlaşdırılmış xəta kodları:

| Qrup | Nümunə kodlar |
|------|---------------|
| General | General.NotFound, General.ServerError |
| Product | Product.NotFound, Product.DuplicateSku |
| Category | Category.NotFound |
| Brand | Brand.NotFound |
| Order | Order.NotFound, Order.EmptyOrder |
| Customer | Customer.NotFound, Customer.DuplicateEmail |
| Authentication | Authentication.InvalidCredentials, Authentication.InactiveUser |
| Validation | Validation.Failed |

---

## Error Response formatları

### Tək xəta

```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "Product.NotFound",
    "message": "Məhsul tapılmadı",
    "type": 3
  }
}
```

### Validasiya xətaları (FluentValidation)

```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "Validation.Failed",
    "message": "Bir və ya bir neçə validasiya xətası baş verdi",
    "type": 2,
    "errors": [
      {
        "code": "Validation.Name",
        "message": "Ad minimum 3 simvol olmalıdır",
        "property": "Name"
      }
    ]
  }
}
```

### Middleware xətası (unhandled exception)

```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "General.ServerError",
    "message": "Server xətası baş verdi. Zəhmət olmasa yenidən cəhd edin.",
    "type": 1
  }
}
```

**Serialization:** camelCase (`JsonNamingPolicy.CamelCase`)

---

## Concurrency error handling

```
Client update → DbUpdateConcurrencyException
  → UnitOfWork.SaveChangesAsync → ConcurrencyException
    → ExceptionHandlingMiddleware → HTTP 409
      → { "code": "Entity.ConcurrencyConflict", "message": "..." }
```

**Retry:** `IUnitOfWork.ReloadAsync(entity)` — entity-ni DB-dən yenidən yükləyir.

---

## Log tipləri xülasəsi

| Tip | Mənbə | Məqsəd |
|-----|-------|--------|
| Information | Serilog, LoggingBehaviour | Normal əməliyyat axını |
| Warning | Controllers, ImageStorage | Validasiya/uğursuz lookup |
| Error | ExceptionHandlingMiddleware | Unhandled exception |
| Fatal | Program.cs | Application startup failure |

---

## Production tövsiyələri

1. **File/Cloud sink** əlavə et (Seq, Elasticsearch, Azure Monitor)
2. **Structured logging** — `{ProductId}` kimi property-lər artıq istifadə olunur
3. **Correlation ID** middleware əlavə et
4. **Health check** endpoint (`/health`)
5. Swagger-ı production-da deaktiv et (və ya auth ilə qoru)
6. Sensitive məlumatları (parol, token) log-lama
