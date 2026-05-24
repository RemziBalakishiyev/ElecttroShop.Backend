# API Sənədləşməsi

**Base URL (Development):** `http://localhost:5223` və ya `https://localhost:7161`

**Swagger UI:** `/swagger`

**Autentifikasiya:** JWT Bearer token — `Authorization: Bearer {accessToken}`

**Səhifələmə formatı (`PagedResult`):**

```json
{
  "isSuccess": true,
  "value": [...],
  "page": 1,
  "pageSize": 10,
  "totalCount": 100,
  "totalPages": 10,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

---

## Auth — `api/Auth`

Controller: `[AllowAnonymous]`

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| POST | `/api/Auth/login` | ❌ | Staff girişi |
| POST | `/api/Auth/refresh-token` | ❌ | Access token yeniləmə |

### POST `/api/Auth/login`

**Request:**
```json
{
  "email": "admin@electroshop.az",
  "password": "Admin123!"
}
```

**Response (`LoginResponseDto`):**
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "base64...",
  "expiresAt": "2026-05-24T12:00:00Z",
  "user": {
    "id": "guid",
    "email": "admin@electroshop.az",
    "fullName": "Administrator",
    "role": "Admin",
    "isActive": true,
    "createdAt": "..."
  }
}
```

### POST `/api/Auth/refresh-token`

**Request:**
```json
{
  "refreshToken": "base64..."
}
```

**Response (`RefreshTokenResponseDto`):** `accessToken`, `refreshToken`, `expiresAt`

---

## Products — `api/Products`

Controller: `//[Authorize]` — **comment edilib, yazma endpoint-ləri açıqdır**

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Products` | ❌ | Səhifələnmiş məhsul siyahısı |
| GET | `/api/Products/{id}` | ❌ | Məhsul detalı |
| GET | `/api/Products/search` | ❌ | Məhsul axtarışı |
| GET | `/api/Products/banner` | ❌ | Banner məhsul |
| GET | `/api/Products/featured` | ❌ | Featured məhsullar (max 5) |
| POST | `/api/Products` | ⚠️ Açıq | Yeni məhsul |
| PUT | `/api/Products/{id}` | ⚠️ Açıq | Məhsul yeniləmə |
| DELETE | `/api/Products/{id}` | ⚠️ Açıq | Soft delete |
| PATCH | `/api/Products/{productId}/price` | ⚠️ Açıq | Qiymət dəyişikliyi |
| PATCH | `/api/Products/{productId}/stock` | ⚠️ Açıq | Stok dəyişikliyi |
| POST | `/api/Products/{productId}/image` | ⚠️ Açıq | Şəkil yükləmə (multipart) |
| POST | `/api/Products/{productId}/banner` | ⚠️ Açıq | Banner təyin et |
| DELETE | `/api/Products/{productId}/banner` | ⚠️ Açıq | Banner-dan çıxar |
| POST | `/api/Products/{productId}/featured` | ⚠️ Açıq | Featured təyin et |
| DELETE | `/api/Products/{productId}/featured` | ⚠️ Açıq | Featured-dan çıxar |
| POST | `/api/Products/{productId}/images` | ⚠️ Açıq | Şəkil ID əlavə et |
| DELETE | `/api/Products/{productId}/images/{imageId}` | ⚠️ Açıq | Şəkil sil |
| POST | `/api/Products/{productId}/images/{imageId}/primary` | ⚠️ Açıq | Əsas şəkil təyin et |
| POST | `/api/Products/{productId}/variants` | ⚠️ Açıq | Variant yarat |
| PUT | `/api/Products/{productId}/variants/{variantId}` | ⚠️ Açıq | Variant yenilə |
| DELETE | `/api/Products/{productId}/variants/{variantId}` | ⚠️ Açıq | Variant deaktiv et |

### GET `/api/Products` — Query parametrləri

| Parametr | Tip | Default | Təsvir |
|----------|-----|---------|--------|
| page | int | 1 | Səhifə nömrəsi |
| pageSize | int | 10 | Səhifə ölçüsü |
| searchTerm | string | null | Axtarış |
| categoryId | guid | null | Kateqoriya filtri |
| brandId | guid | null | Brend filtri |
| minPrice | decimal | null | Min qiymət |
| maxPrice | decimal | null | Max qiymət |
| isActive | bool | null | Aktivlik filtri |

**Response:** `PagedResult<ProductListDto>` — `finalDiscountPercent`, `finalPrice` daxildir

### POST `/api/Products` — Request (`CreateProductCommand`)

```json
{
  "name": "iPhone 15 Pro",
  "description": "...",
  "price": 2500.00,
  "currency": "AZN",
  "sku": "IPH15PRO-256",
  "categoryId": "guid",
  "brandId": "guid",
  "vatRate": 0.18,
  "stock": 50,
  "imageIds": ["guid1", "guid2"],
  "variants": [
    { "attributesJson": "{\"color\":\"Black\"}", "imageId": null, "isActive": true }
  ]
}
```

**Response:** `ProductDto` (RowVersion, endirimli qiymət daxildir)

### PATCH `/api/Products/{productId}/stock`

```json
{ "quantityChange": -5 }
```

---

## Categories — `api/Categories`

Controller: `[Authorize]` — oxuma endpoint-ləri `[AllowAnonymous]`

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Categories` | ❌ | Səhifələnmiş kateqoriya siyahısı |
| GET | `/api/Categories/{id}` | ❌ | ID ilə kateqoriya |
| GET | `/api/Categories/slug/{slug}` | ❌ | Slug ilə kateqoriya |
| GET | `/api/Categories/lookup` | ❌ | Lookup (cache ilə) |
| POST | `/api/Categories` | ✅ | Kateqoriya yarat |
| PUT | `/api/Categories/{id}` | ✅ | Kateqoriya yenilə |
| DELETE | `/api/Categories/{id}` | ✅ | Soft delete |
| GET | `/api/Categories/{categoryId}/attributes` | ✅ | Kateqoriya atributları |
| POST | `/api/Categories/{categoryId}/attributes` | ✅ | Atribut yarat |
| PUT | `/api/Categories/attributes/{id}` | ✅ | Atribut yenilə |
| DELETE | `/api/Categories/attributes/{id}` | ✅ | Atribut sil |
| POST | `/api/Categories/attributes/{attributeId}/values` | ✅ | Atribut dəyəri əlavə et |
| PUT | `/api/Categories/attributes/values/{id}` | ✅ | Atribut dəyəri yenilə |
| DELETE | `/api/Categories/attributes/values/{id}` | ✅ | Atribut dəyəri sil |

---

## Brands — `api/Brands`

Controller: `[Authorize]` — oxuma endpoint-ləri `[AllowAnonymous]`

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Brands` | ❌ | Səhifələnmiş brend siyahısı |
| GET | `/api/Brands/{id}` | ❌ | Brend detalı |
| GET | `/api/Brands/promotional` | ❌ | Promotional brendlər (max 4) |
| GET | `/api/Brands/lookup` | ❌ | Lookup (cache ilə) |
| POST | `/api/Brands` | ✅ | Brend yarat |
| PUT | `/api/Brands/{id}` | ✅ | Brend yenilə |
| DELETE | `/api/Brands/{id}` | ✅ | Soft delete |

---

## Customers — `api/Customers`

Controller: `[Authorize]`

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Customers/{id}` | ✅ | Müştəri detalı |
| GET | `/api/Customers/email/{email}` | ✅ | Email ilə müştəri |
| POST | `/api/Customers/register` | ❌ | Müştəri qeydiyyatı |
| PUT | `/api/Customers/{id}` | ✅ | Müştəri yenilə |

### POST `/api/Customers/register`

```json
{
  "fullName": "Ali Məmmədov",
  "email": "ali@example.com",
  "phone": "+994501234567"
}
```

**Qeyd:** Müştəri qeydiyyatında parol və JWT verilmir.

---

## Orders — `api/Orders`

Controller: `[Authorize]` — bütün endpoint-lər auth tələb edir

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Orders/{id}` | ✅ | Sifariş detalı |
| GET | `/api/Orders/customer/{customerId}` | ✅ | Müştərinin sifarişləri |
| POST | `/api/Orders` | ✅ | Sifariş yarat |
| POST | `/api/Orders/{orderId}/items` | ✅ | Sifarişə məhsul əlavə et |
| DELETE | `/api/Orders/{orderId}/items/{productId}` | ✅ | Sifarişdən məhsul sil |
| PATCH | `/api/Orders/{id}/mark-paid` | ✅ | Sifarişi ödənilmiş qeyd et |

### POST `/api/Orders`

```json
{ "customerId": "guid" }
```

### POST `/api/Orders/{orderId}/items`

```json
{
  "productId": "guid",
  "quantity": 2
}
```

---

## Discounts — `api/discounts`

Controller: `[Authorize]` — bütün endpoint-lər auth tələb edir

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/discounts` | ✅ | Endirim siyahısı |
| GET | `/api/discounts/{id}` | ✅ | Endirim detalı |
| POST | `/api/discounts` | ✅ | Endirim yarat |
| PUT | `/api/discounts/{id}` | ✅ | Endirim yenilə |
| DELETE | `/api/discounts/{id}` | ✅ | Endirim sil (deaktiv) |

### POST `/api/discounts`

```json
{
  "type": "Product",
  "productId": "guid",
  "brandId": null,
  "categoryId": null,
  "percent": 15.0,
  "startDate": "2026-01-01T00:00:00Z",
  "endDate": "2026-12-31T23:59:59Z",
  "isActive": true
}
```

**DiscountType:** `Product`, `Brand`, `Category`

---

## Dashboard — `api/dashboard`

Controller: `//[Authorize]` — **comment edilib, açıqdır**

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/dashboard` | ⚠️ Açıq | Dashboard statistikası |
| GET | `/api/dashboard/chart` | ⚠️ Açıq | Qrafik məlumatları |

### GET `/api/dashboard/chart` — Query

| Parametr | Default | Dəyərlər |
|----------|---------|----------|
| period | monthly | daily, weekly, monthly |
| periodCount | 12 | Period sayı |

---

## Images — `api/Images`

Controller: Auth attribute yoxdur

| Method | Route | Auth | Təsvir |
|--------|-------|------|--------|
| GET | `/api/Images/{imageId}.{extension}` | ❌ | Şəkil oxuma |
| GET | `/api/Images/{imageId}` | ❌ | Şəkil oxuma (extension olmadan) |
| POST | `/api/Images/upload` | ⚠️ Açıq | Şəkil yükləmə (multipart) |

**POST Response:** `{ "imageId": "guid" }`

**Dəstəklənən formatlar:** `.jpg`, `.jpeg`, `.png`, `.webp`, `.gif` (max 10 MB)

---

## Xəta response formatı

### Tək xəta (`Result.Failure`)

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

**ErrorType:** 0=None, 1=Failure, 2=Validation, 3=NotFound, 4=Conflict, 5=Unauthorized, 6=Forbidden

### Validasiya xətaları

```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "Validation.Failed",
    "message": "Bir və ya bir neçə validasiya xətası baş verdi",
    "type": 2,
    "errors": [
      { "code": "Validation.Name", "message": "...", "property": "Name" }
    ]
  }
}
```

---

## HTTP Status kodları

| Status | Səbəb |
|--------|-------|
| 200 | Uğur |
| 400 | Validasiya xətası |
| 401 | Autentifikasiya tələb olunur |
| 403 | İcazə yoxdur |
| 404 | Tapılmadı |
| 409 | Concurrency conflict |
| 500 | Server xətası |
