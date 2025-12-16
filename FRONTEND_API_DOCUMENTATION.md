# Front-End API Sənədi - Backend Dəyişiklikləri

## Məzmun
1. [Primary Image URL](#primary-image-url)
2. [Primary Image Dəyişdirmə API](#primary-image-dəyişdirmə-api)
3. [Şəkil Yükləmə və Oxuma Workflow](#şəkil-yükləmə-və-oxuma-workflow)
4. [Lookup API-ləri (Library Cədvəli)](#lookup-api-ləri-library-cədvəli)

---

## Primary Image URL

### Backend Dəyişiklikləri

**Nə dəyişdi:**
- `GetProducts` və `GetProductById` API-lərindən qayıdan response-da `PrimaryImageUrl` field-i **avtomatik olaraq set edilir**
- Extension (`.jpg`, `.png` və s.) **avtomatik əlavə edilir**

**Məntiq:**
1. Əgər primary şəkil varsa (`IsPrimary = true`), o istifadə olunur
2. Əgər primary şəkil yoxdursa, ilk şəkil (DisplayOrder-a görə) istifadə olunur
3. Şəkil extension-ı avtomatik tapılır və URL-ə əlavə edilir
4. Əgər heç bir şəkil yoxdursa, `null` qaytarılır

### Response Formatı

#### ProductListDto (GetProducts)
```json
{
  "id": "guid",
  "name": "string",
  "price": 0.00,
  "currency": "AZN",
  "sku": "string",
  "categoryName": "string",
  "brandName": "string",
  "stock": 0,
  "isActive": true,
  "primaryImageUrl": "/api/images/{imageId}.jpg",  // Extension ilə avtomatik set edilir
  "isBanner": false,
  "isFeatured": false,
  "displayOrder": null,
  "finalDiscountPercent": 0,
  "finalPrice": 0.00
}
```

#### ProductDto (GetProductById)
```json
{
  "id": "guid",
  "name": "string",
  "description": "string",
  "price": 0.00,
  "currency": "AZN",
  "sku": "string",
  "categoryId": "guid",
  "categoryName": "string",
  "brandId": "guid",
  "brandName": "string",
  "vatRate": 0.18,
  "stock": 0,
  "isActive": true,
  "images": [
    {
      "id": "guid",
      "imageId": "guid",
      "imageUrl": "/api/images/{imageId}.jpg",
      "displayOrder": 0,
      "isPrimary": true
    }
  ],
  "primaryImageUrl": "/api/images/{imageId}.jpg",  // Extension ilə avtomatik set edilir
  "isBanner": false,
  "isFeatured": false,
  "displayOrder": null,
  "finalDiscountPercent": 0,
  "finalPrice": 0.00,
  "categoryAttributes": [],
  "variants": [],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

**Qeyd:** Front-end-də heç bir əlavə məntiq yazmağa ehtiyac yoxdur. Sadəcə `primaryImageUrl` field-ini istifadə edin.

---

## Primary Image Dəyişdirmə API

### Backend Dəyişiklikləri

**Nə dəyişdi:**
- API artıq mövcuddur və işləyir
- Primary image silinmir, yalnız `IsPrimary` statusu dəyişir (concurrency problemi aradan qaldırıldı)

### Endpoint
```
POST /api/products/{productId}/images/{imageId}/primary
```

### Request Headers
```
Authorization: Bearer {token}  // Tələb olunur
```

### Path Parameters
- `productId` (Guid) - Məhsul ID-si
- `imageId` (Guid) - Primary olaraq təyin ediləcək şəkil ID-si

### Uğurlu Response (200 OK)
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

### Xətalar

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 400 | `Validation.Failed` | Validasiya xətası |
| 401 | `Unauthorized` | "Unauthorized" |
| 404 | `Product.NotFound` | "Məhsul tapılmadı" |
| 500 | `Error.Failure` | "Daxili server xətası" |

**Qeyd:** Uğurlu olduqdan sonra məhsul məlumatlarını yenidən yükləyin (`GetProductById` çağırın) ki, `primaryImageUrl` yenilənsin.

---

## Şəkil Yükləmə və Oxuma Workflow

### Backend Dəyişiklikləri

**Nə dəyişdi:**
1. **Yeni Standalone Image Upload API** əlavə edildi
2. **Şəkil oxuma endpoint-i** əlavə edildi (extension ilə və olmadan)
3. **Workflow dəyişdi:** Əvvəlcə şəkilləri yükləyin, sonra məhsul yaradın

### 1. Şəkil Yükləmə API (Standalone)

**Endpoint:**
```
POST /api/images/upload
```

**Request Headers:**
```
Authorization: Bearer {token}  // Tələb olunur
Content-Type: multipart/form-data
```

**Request Body:**
- `file` (IFormFile) - Yüklənəcək şəkil faylı

**Uğurlu Response (200 OK):**
```json
{
  "imageId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

**Xətalar:**

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 400 | `Image.Required` | "Şəkil faylı tələb olunur" |
| 400 | `ImageStream.Required` | "Şəkil stream-i tələb olunur" |
| 401 | `Unauthorized` | "Unauthorized" |
| 500 | `Error.Failure` | "Daxili server xətası" |

**Qeyd:** Hər şəkil üçün ayrı-ayrı request göndərin və `imageId`-ləri toplayın.

### 2. Şəkil Oxuma API

**Endpoint-lər:**
```
GET /api/images/{imageId}.{extension}  // Extension ilə (tövsiyə olunur)
GET /api/images/{imageId}              // Extension olmadan (backward compatibility)
```

**Nümunələr:**
- `GET /api/images/dc60ac7b-1672-445c-ac03-f6235a035256.jpg`
- `GET /api/images/dc60ac7b-1672-445c-ac03-f6235a035256`

**Request Headers:**
```
// Authorization tələb olunmur (AllowAnonymous)
```

**Response:**
- Content-Type: `image/jpeg`, `image/png`, `image/webp` və s.
- Status Code: `200 OK` (şəkil tapıldı) və ya `404 Not Found` (şəkil tapılmadı)

**Qeyd:** Bu endpoint public-dir və şəkilləri birbaşa göstərmək üçün istifadə edilə bilər.

### 3. Məhsul Yaradılması (Şəkillərlə)

**Endpoint:**
```
POST /api/products
```

**Request Body:**
```json
{
  "name": "string",
  "description": "string",
  "price": 0.00,
  "currency": "AZN",
  "sku": "string",
  "categoryId": "guid",
  "brandId": "guid",
  "vatRate": 0.18,
  "stock": 0,
  "imageIds": [
    "guid-1",
    "guid-2",
    "guid-3"
  ],
  "variants": [
    {
      "imageId": "guid",  // Opsional - variant üçün şəkil
      "attributes": {
        "Rəng": "Qara",
        "Ölçü": "128GB"
      }
    }
  ]
}
```

**Field Açıqlamaları:**

#### `imageIds` (List<Guid>)
- **Məcburi:** Xeyr (boş array ola bilər)
- **Təsvir:** Əvvəlcə `/api/images/upload` endpoint-indən alınmış `imageId`-lərin array-i
- **Sıra:** İlk element (index 0) avtomatik olaraq **primary şəkil** olur
- **Nümunə:** `["guid-1", "guid-2", "guid-3"]` → `guid-1` primary olur

#### `variants[].imageId` (Guid?)
- **Məcburi:** Xeyr (null ola bilər)
- **Təsvir:** Variant üçün xüsusi şəkil (əgər variantın öz şəkili varsa)
- **Qeyd:** Bu `imageId` də əvvəlcə `/api/images/upload` ilə yüklənməlidir

### Workflow

**MƏCBURİ SIRA:**
1. **Əvvəlcə şəkilləri yükləyin** → `POST /api/images/upload` → `imageId`-ləri alın
2. **Sonra məhsul yaradın** → `POST /api/products` → `imageIds` array-ində göndərin

**Nümunə Workflow:**
```
1. POST /api/images/upload → { imageId: "guid-1" }
2. POST /api/images/upload → { imageId: "guid-2" }
3. POST /api/images/upload → { imageId: "guid-3" }
4. imageIds = ["guid-1", "guid-2", "guid-3"] topla
5. POST /api/products → { ..., imageIds: ["guid-1", "guid-2", "guid-3"] }
```

**Qeyd:** Məhsul yaratmadan əvvəl şəkilləri yükləmək məcburidir.

---

## Lookup API-ləri (Library Cədvəli)

### Backend Dəyişiklikləri

**Nə dəyişdi:**
- Yeni **cached lookup API-ləri** əlavə edildi
- Categories və Brands üçün **key-value** formatında məlumat qaytarır
- **Cache müddəti:** 1 saat (backend-də `IMemoryCache` istifadə olunur)
- Select boxlar üçün nəzərdə tutulub (page API-ləri əvəzinə)

### Təsvir

Lookup API-ləri **cache management** ilə işləyir və **key-value** formatında məlumat qaytarır. Bu API-lər select boxlar və dropdown-lar üçün nəzərdə tutulub. Hər dəfə page API-lərini çağırmamaq üçün cache-dən istifadə edilir.

**Cache müddəti:** 1 saat

### Response Formatı
```json
{
  "items": [
    {
      "key": "guid-string",  // ID (string formatında)
      "value": "Display Name"  // Görünən ad
    }
  ],
  "cachedAt": "2024-01-15T10:30:00Z",
  "cacheKey": "CategoriesLookup"
}
```

---

## Categories Lookup API

### Endpoint
```
GET /api/categories/lookup
```

### Request Headers
```
Authorization: Bearer {token}  // Opsional (AllowAnonymous)
```

### Uğurlu Response (200 OK)
```json
{
  "items": [
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "value": "Smartfonlar"
    },
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "value": "Notebooklar"
    },
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
      "value": "Televizorlar"
    }
  ],
  "cachedAt": "2024-01-15T10:30:00Z",
  "cacheKey": "CategoriesLookup"
}
```

### Xətalar

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 500 | `Error.Failure` | "Daxili server xətası" |

**Qeyd:** 
- Səhifələmə yoxdur, bütün məlumat bir request-də gəlir
- Yalnız aktiv kateqoriyalar qaytarılır (`IsActive = true` və `IsDeleted = false`)
- Ad-a görə artan sırada sıralanır

---

## Brands Lookup API

### Endpoint
```
GET /api/brands/lookup
```

### Request Headers
```
Authorization: Bearer {token}  // Opsional (AllowAnonymous)
```

### Uğurlu Response (200 OK)
```json
{
  "items": [
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "value": "Apple"
    },
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
      "value": "Samsung"
    },
    {
      "key": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
      "value": "Sony"
    }
  ],
  "cachedAt": "2024-01-15T10:30:00Z",
  "cacheKey": "BrandsLookup"
}
```

### Xətalar

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 500 | `Error.Failure` | "Daxili server xətası" |

**Qeyd:** 
- Səhifələmə yoxdur, bütün məlumat bir request-də gəlir
- Yalnız aktiv brendlər qaytarılır (`IsActive = true` və `IsDeleted = false`)
- Ad-a görə artan sırada sıralanır

---

## Ümumi Qeydlər

### 1. Primary Image URL
- Backend avtomatik set edir, siz sadəcə istifadə edin
- Extension avtomatik əlavə edilir
- `null` ola bilər, buna görə null check edin

### 2. Şəkil Yükləmə Workflow
- **MƏCBURİ SIRA:** Əvvəlcə şəkilləri yükləyin, sonra məhsul yaradın
- Hər şəkil üçün ayrı-ayrı request göndərin
- `imageId`-ləri toplayın və `CreateProduct`-də göndərin
- İlk şəkil (index 0) avtomatik primary olur

### 3. Şəkil Oxuma
- Extension ilə və olmadan işləyir
- Public endpoint-dir (AllowAnonymous)
- Birbaşa `<img src="/api/images/{imageId}.jpg">` kimi istifadə edə bilərsiniz

### 4. Lookup API-ləri
- Səhifələmə yoxdur, bütün məlumat bir request-də gəlir
- Cache ilə işləyir, performans yüksəkdir
- Select boxlar üçün nəzərdə tutulub
- **QEYD:** Page API-lərini çağırmayın, lookup API-lərini istifadə edin

### 5. Authorization
- Primary Image dəyişdirmə üçün token tələb olunur
- Şəkil yükləmə üçün token tələb olunur
- Şəkil oxuma üçün token tələb olunmur (AllowAnonymous)
- Lookup API-ləri üçün token opsionaldır (AllowAnonymous)

### 6. Error Handling
- Bütün API-lərdə error response formatı eynidir
- `error.code` və `error.message` field-lərini istifadə edin

---

**Son yenilənmə:** 2024-12-16

