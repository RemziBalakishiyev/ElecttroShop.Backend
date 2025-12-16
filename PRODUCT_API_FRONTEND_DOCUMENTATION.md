# Product API - Front-End Sənədi

## Məzmun
1. [Product Variant-da SKU və Qiymət](#product-variant-da-sku-və-qiymət)
2. [Product Yaratma API](#product-yaratma-api)
3. [Product Şəkil Yükləmə API](#product-şəkil-yükləmə-api)

---

## Product Variant-da SKU və Qiymət

### Variant Strukturu

**Product Variant** sistemi müxtəlif atribut kombinasiyaları olan məhsullar üçün nəzərdə tutulub. **Variantlar sadəcə atribut fərqlərini təmsil edir.**

**Misal:**
- **Product:** iPhone 12
  - **SKU:** `IPHONE12-001` (Product səviyyəsində)
  - **Qiymət:** 2500 AZN (Product səviyyəsində)
  - **Stok:** 10 ədəd (Product səviyyəsində)
  - **Variant 1:** Black - Atributlar: `{"Color": "Black"}`
  - **Variant 2:** White - Atributlar: `{"Color": "White"}`
  - **Variant 3:** Blue - Atributlar: `{"Color": "Blue"}`

**Qeyd:**
- **SKU, Price və Stock** yalnız **Product səviyyəsindədir**
- **Variantlar** yalnız **atributları** (rəng, ölçü və s.) təmsil edir
- Bütün variantlar eyni Product-ın SKU, Price və Stock dəyərlərini paylaşır
- Variant yaradarkən yalnız **Attributes** və **ImageId** (opsional) tələb olunur

---

## Product Yaratma API

### Endpoint
```
POST /api/products
```

### Request Headers
```
Content-Type: application/json
Authorization: Bearer {token}  // Tələb olunur
```

### Request Body

```json
{
  "name": "string (tələb olunur, 3-200 simvol)",
  "description": "string (opsional, maksimum 2000 simvol)",
  "price": "decimal (tələb olunur, > 0, <= 1,000,000)",
  "currency": "string (tələb olunur, 3 simvol, məs: AZN, TRY, USD, EUR)",
  "sku": "string (tələb olunur, 3-50 simvol, unikal olmalıdır)",
  "categoryId": "Guid (tələb olunur)",
  "brandId": "Guid (tələb olunur)",
  "vatRate": "decimal (tələb olunur, 0-1 arası, məs: 0.18)",
  "stock": "integer (tələb olunur, >= 0)",
  "imageIds": "Guid[] (opsional, şəkil ID-ləri)",
  "variants": [
    {
      "imageId": "Guid? (opsional, variant üçün xüsusi şəkil)",
      "attributes": {
        "Color": "Black",
        "Storage": "256GB"
      }
    }
  ]
}
```

### Request Nümunəsi

```json
{
  "name": "iPhone 14 Pro",
  "description": "Apple-ın ən yeni flagship telefonu",
  "price": 2500.00,
  "currency": "AZN",
  "sku": "IPHONE14PRO-BASE",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "brandId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "vatRate": 0.18,
  "stock": 100,
  "imageIds": [
    "3fa85f64-5717-4562-b3fc-2c963f66afa8"
  ],
  "variants": [
    {
      "imageId": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
      "attributes": {
        "Color": "Black"
      }
    },
    {
      "attributes": {
        "Color": "White"
      }
    },
    {
      "attributes": {
        "Color": "Blue"
      }
    }
  ]
}
```

### Uğurlu Response (200 OK)

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "iPhone 14 Pro",
  "description": "Apple-ın ən yeni flagship telefonu",
  "price": 2500.00,
  "currency": "AZN",
  "sku": "IPHONE14PRO-BASE",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "categoryName": "Smartfonlar",
  "brandId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "brandName": "Apple",
  "vatRate": 0.18,
  "stock": 100,
  "isActive": true,
  "images": [],
  "isBanner": false,
  "isFeatured": false,
  "displayOrder": null,
  "finalDiscountPercent": 0,
  "finalPrice": 2500.00,
  "categoryAttributes": [],
  "variants": [],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": null
}
```

### Validasiya Xətaları (400 Bad Request)

#### Format 1: FluentValidation Xətaları (Çoxlu xəta)

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
        "message": "Məhsulun adı boş ola bilməz",
        "property": "Name"
      },
      {
        "code": "Validation.Sku",
        "message": "Bu SKU artıq istifadə olunur",
        "property": "Sku"
      }
    ]
  }
}
```

#### Format 2: Tək Xəta

```json
{
  "code": "Product.InvalidData",
  "message": "SKU boş ola bilməz",
  "type": 2
}
```

### Validasiya Mesajları

| Field | Xəta Mesajı | Şərt |
|-------|-------------|------|
| `name` | "Məhsulun adı boş ola bilməz" | Boş ola bilməz |
| `name` | "Məhsulun adı minimum 3 simvol olmalıdır" | Minimum 3 simvol |
| `name` | "Məhsulun adı maksimum 200 simvol ola bilər" | Maksimum 200 simvol |
| `description` | "Məhsulun təsviri maksimum 2000 simvol ola bilər" | Maksimum 2000 simvol |
| `price` | "Qiymət 0-dan böyük olmalıdır" | > 0 |
| `price` | "Qiymət 1,000,000-dan kiçik və ya bərabər olmalıdır" | <= 1,000,000 |
| `currency` | "Valyuta boş ola bilməz" | Boş ola bilməz |
| `currency` | "Valyuta 3 simvol olmalıdır (məs: TRY, USD, EUR)" | 3 simvol |
| `currency` | "Yanlış valyuta. Etibarlı valyutalar: AZN, TRY, USD, EUR" | Etibarlı valyuta olmalıdır |
| `sku` | "SKU boş ola bilməz" | Boş ola bilməz |
| `sku` | "SKU minimum 3 simvol olmalıdır" | Minimum 3 simvol |
| `sku` | "SKU maksimum 50 simvol ola bilər" | Maksimum 50 simvol |
| `sku` | "SKU yalnız böyük hərflər, rəqəmlər, tire və alt xətt simvollarından ibarət ola bilər" | Format: `^[A-Z0-9\-_]+$` |
| `sku` | "Bu SKU artıq istifadə olunur" | Unikal olmalıdır |
| `categoryId` | "Kateqoriya seçilməlidir" | Boş ola bilməz |
| `categoryId` | "Seçilmiş kateqoriya tapılmadı" | Mövcud olmalıdır |
| `brandId` | "Brend seçilməlidir" | Boş ola bilməz |
| `brandId` | "Seçilmiş brend tapılmadı" | Mövcud olmalıdır |
| `vatRate` | "ƏDV dərəcəsi 0 ilə 1 arasında olmalıdır (məs: 0.18)" | 0-1 arası |
| `stock` | "Stok miqdarı mənfi ola bilməz" | >= 0 |

### Digər Xətalar

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 401 | `Unauthorized` | "Unauthorized" |
| 404 | `Product.NotFound` | "Məhsul tapılmadı" |
| 500 | `Error.Failure` | "Daxili server xətası" |

---

## Product Şəkil Yükləmə API

### Endpoint
```
POST /api/products/{productId}/image
```

### Request Headers
```
Content-Type: multipart/form-data
Authorization: Bearer {token}  // Tələb olunur
```

### Request Body (Form Data)

| Field | Type | Tələb | Təsvir |
|-------|------|-------|--------|
| `file` | File | Bəli | Şəkil faylı |

### Request Nümunəsi (JavaScript/Fetch)

```javascript
const formData = new FormData();
formData.append('file', fileInput.files[0]);

fetch(`/api/products/${productId}/image`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});
```

### Uğurlu Response (200 OK)

```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "name": "iPhone 14 Pro",
  "description": "Apple-ın ən yeni flagship telefonu",
  "price": 2500.00,
  "currency": "AZN",
  "sku": "IPHONE14PRO-BASE",
  "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "categoryName": "Smartfonlar",
  "brandId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
  "brandName": "Apple",
  "vatRate": 0.18,
  "stock": 100,
  "isActive": true,
  "images": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa8",
      "imageId": "3fa85f64-5717-4562-b3fc-2c963f66afa9",
      "imageUrl": "/api/images/3fa85f64-5717-4562-b3fc-2c963f66afa9",
      "displayOrder": 0,
      "isPrimary": true
    }
  ],
  "isBanner": false,
  "isFeatured": false,
  "displayOrder": null,
  "finalDiscountPercent": 0,
  "finalPrice": 2500.00,
  "categoryAttributes": [],
  "variants": [],
  "createdAt": "2024-01-15T10:30:00Z",
  "updatedAt": "2024-01-15T10:35:00Z"
}
```

### Validasiya Xətaları (400 Bad Request)

#### Format 1: Fayl Boşdur

```json
{
  "code": "Image.Required",
  "message": "Şəkil faylı tələb olunur",
  "type": 2
}
```

#### Format 2: FluentValidation Xətaları

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
        "code": "Validation.ProductId",
        "message": "Məhsul tapılmadı",
        "property": "ProductId"
      },
      {
        "code": "Validation.FileName",
        "message": "İcazə verilən fayl formatları: .jpg, .jpeg, .png, .webp, .gif",
        "property": "FileName"
      }
    ]
  }
}
```

### Validasiya Mesajları

| Field | Xəta Mesajı | Şərt |
|-------|-------------|------|
| `file` | "Şəkil faylı tələb olunur" | Fayl göndərilməlidir |
| `file` | "File is empty" | Fayl boş ola bilməz |
| `productId` | "Məhsul ID-si boş ola bilməz" | Boş ola bilməz |
| `productId` | "Məhsul tapılmadı" | Mövcud olmalıdır |
| `fileName` | "Fayl adı boş ola bilməz" | Boş ola bilməz |
| `fileName` | "İcazə verilən fayl formatları: .jpg, .jpeg, .png, .webp, .gif" | Yalnız bu formatlar |
| `contentType` | "Content type boş ola bilməz" | Boş ola bilməz |
| `contentType` | "İcazə verilən content type-lar: image/jpeg, image/jpg, image/png, image/webp, image/gif" | Yalnız bu content type-lar |
| `imageStream` | "Şəkil stream-i tələb olunur" | Stream mövcud olmalıdır |

### Fayl Tələbləri

- **Maksimum ölçü:** 10 MB
- **İcazə verilən formatlar:**
  - `.jpg` / `.jpeg` (image/jpeg, image/jpg)
  - `.png` (image/png)
  - `.webp` (image/webp)
  - `.gif` (image/gif)

### Qeydlər

1. **Primary Image:** Yeni şəkil yüklənəndə, əgər mövcud primary şəkil varsa, o silinir və yeni şəkil primary olaraq təyin edilir.

2. **Display Order:** Yeni şəkil avtomatik olaraq ən yüksək display order + 1 dəyəri alır.

3. **Şəkil Görüntüləmə:** Yüklənmiş şəkillər `/api/images/{imageId}` endpoint-indən əldə edilə bilər.

### Digər Xətalar

| Status Code | Xəta Kodu | Mesaj |
|-------------|-----------|-------|
| 401 | `Unauthorized` | "Unauthorized" |
| 404 | `Product.NotFound` | "Məhsul tapılmadı" |
| 500 | `Error.Failure` | "Daxili server xətası" |

---

## Ümumi Qeydlər

### Error Response Strukturu

#### Tək Xəta
```json
{
  "code": "Error.Code",
  "message": "Xəta mesajı",
  "type": 2  // ErrorType enum: 0=None, 1=Failure, 2=Validation, 3=NotFound, 4=Conflict, 5=Unauthorized, 6=Forbidden
}
```

#### Çoxlu Xəta (Validation)
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
        "code": "Validation.PropertyName",
        "message": "Xəta mesajı",
        "property": "PropertyName"
      }
    ]
  }
}
```

### SKU Formatı

- **Minimum uzunluq:** 3 simvol
- **Maksimum uzunluq:** 50 simvol
- **Format:** Yalnız böyük hərflər, rəqəmlər, tire (`-`) və alt xətt (`_`)
- **Regex:** `^[A-Z0-9\-_]+$`
- **Unikal:** Hər SKU unikal olmalıdır
- **Avtomatik normalizasiya:** Kiçik hərflər böyük hərflərə çevrilir, boşluqlar silinir

### Valyuta Formatı

- **Uzunluq:** 3 simvol
- **Etibarlı valyutalar:** `AZN`, `TRY`, `USD`, `EUR`
- **Case-sensitive:** Yox (avtomatik böyük hərfə çevrilir)

### Date Formatı

- **Format:** ISO 8601 (UTC)
- **Nümunə:** `2024-01-15T10:30:00Z`

---

## Front-End İmplementasiya Nümunələri

### Product Yaratma (React/TypeScript)

```typescript
interface CreateProductRequest {
  name: string;
  description?: string;
  price: number;
  currency: string;
  sku: string;
  categoryId: string;
  brandId: string;
  vatRate: number;
  stock: number;
  imageIds?: string[];
  variants?: {
    sku: string;
    price: number;
    currency: string;
    stock: number;
    imageId?: string;
    attributes: Record<string, string>;
  }[];
}

interface ValidationError {
  code: string;
  message: string;
  property: string;
}

interface ErrorResponse {
  isSuccess: false;
  isFailure: true;
  error: {
    code: string;
    message: string;
    type: number;
    errors?: ValidationError[];
  };
}

async function createProduct(
  data: CreateProductRequest,
  token: string
): Promise<ProductDto> {
  const response = await fetch('/api/products', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    },
    body: JSON.stringify(data)
  });

  if (!response.ok) {
    const error: ErrorResponse = await response.json();
    
    // Validasiya xətalarını göstər
    if (error.error.errors) {
      error.error.errors.forEach(err => {
        console.error(`${err.property}: ${err.message}`);
      });
    } else {
      console.error(error.error.message);
    }
    
    throw new Error(error.error.message);
  }

  return await response.json();
}
```

### Şəkil Yükləmə (React/TypeScript)

```typescript
async function uploadProductImage(
  productId: string,
  file: File,
  token: string
): Promise<ProductDto> {
  // Fayl validasiyası (front-end-də)
  const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/webp', 'image/gif'];
  const maxSize = 10 * 1024 * 1024; // 10 MB

  if (!allowedTypes.includes(file.type)) {
    throw new Error('İcazə verilən fayl formatları: .jpg, .jpeg, .png, .webp, .gif');
  }

  if (file.size > maxSize) {
    throw new Error('Fayl ölçüsü 10 MB-dan böyük ola bilməz');
  }

  const formData = new FormData();
  formData.append('file', file);

  const response = await fetch(`/api/products/${productId}/image`, {
    method: 'POST',
    headers: {
      'Authorization': `Bearer ${token}`
    },
    body: formData
  });

  if (!response.ok) {
    const error: ErrorResponse = await response.json();
    
    if (error.error.errors) {
      error.error.errors.forEach(err => {
        console.error(`${err.property}: ${err.message}`);
      });
    } else {
      console.error(error.error.message);
    }
    
    throw new Error(error.error.message);
  }

  return await response.json();
}
```

---

## Test Nümunələri

### 1. Product Yaratma - Uğurlu

```bash
curl -X POST "https://api.example.com/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "Test Product",
    "price": 100.00,
    "currency": "AZN",
    "sku": "TEST-001",
    "categoryId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "brandId": "3fa85f64-5717-4562-b3fc-2c963f66afa7",
    "vatRate": 0.18,
    "stock": 10
  }'
```

### 2. Product Yaratma - Validasiya Xətası

```bash
curl -X POST "https://api.example.com/api/products" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "name": "",
    "price": -10,
    "sku": "AB"
  }'
```

**Response:**
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
        "message": "Məhsulun adı boş ola bilməz",
        "property": "Name"
      },
      {
        "code": "Validation.Price",
        "message": "Qiymət 0-dan böyük olmalıdır",
        "property": "Price"
      },
      {
        "code": "Validation.Sku",
        "message": "SKU minimum 3 simvol olmalıdır",
        "property": "Sku"
      }
    ]
  }
}
```

### 3. Şəkil Yükləmə - Uğurlu

```bash
curl -X POST "https://api.example.com/api/products/3fa85f64-5717-4562-b3fc-2c963f66afa6/image" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "file=@/path/to/image.jpg"
```

### 4. Şəkil Yükləmə - Yanlış Format

```bash
curl -X POST "https://api.example.com/api/products/3fa85f64-5717-4562-b3fc-2c963f66afa6/image" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "file=@/path/to/document.pdf"
```

**Response:**
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
        "code": "Validation.FileName",
        "message": "İcazə verilən fayl formatları: .jpg, .jpeg, .png, .webp, .gif",
        "property": "FileName"
      }
    ]
  }
}
```

---

**Son yenilənmə:** 2024-01-15

