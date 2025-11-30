# ElectroShop Admin Side API Dokumentasiyası

## 📋 Mündəricat

1. [Ümumi Məlumat](#ümumi-məlumat)
2. [Authentication](#authentication)
3. [Məhsul İdarəetməsi (Product Management)](#məhsul-idarəetməsi-product-management)
4. [Banner və Featured Məhsul İdarəetməsi](#banner-və-featured-məhsul-idarəetməsi)
5. [Error Handling](#error-handling)

---

## Ümumi Məlumat

### Base URL
```
https://your-api-domain.com/api
```

### Authentication
Bütün admin endpoint-ləri authentication tələb edir. Request header-da JWT token göndərilməlidir:
```
Authorization: Bearer {accessToken}
```

### Response Format
Bütün API response-ları aşağıdakı formatda qaytarılır:

**Uğurlu Response:**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": { /* data */ }
}
```

**Xəta Response:**
```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "Error.Code",
    "message": "Xəta mesajı",
    "type": 2
  }
}
```

---

## Authentication

Admin panel üçün authentication prosesi user side ilə eynidir. Ətraflı məlumat üçün `USER_SIDE_API_DOCUMENTATION.md` faylına baxın.

---

## Məhsul İdarəetməsi (Product Management)

### 1. Məhsul Yaratmaq

**Endpoint:** `POST /api/products`

**Authentication:** Tələb olunur (JWT Token)

**Request Body:**
```json
{
  "name": "iPhone 15 Pro Max",
  "description": "Məhsul təsviri",
  "price": 5000.00,
  "currency": "AZN",
  "sku": "IPHONE-15-PRO-MAX-256",
  "categoryId": "guid",
  "brandId": "guid",
  "vatRate": 18.00,
  "stock": 50
}
```

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": {
    "id": "guid",
    "name": "iPhone 15 Pro Max",
    "price": 5000.00,
    "currency": "AZN",
    "sku": "IPHONE-15-PRO-MAX-256",
    "categoryName": "Smartfonlar",
    "brandName": "Apple",
    "stock": 50,
    "isActive": true,
    "imageUrl": null,
    "finalDiscountPercent": 0,
    "finalPrice": 5000.00
  }
}
```

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');

const response = await fetch('https://your-api-domain.com/api/products', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${accessToken}`
  },
  body: JSON.stringify({
    name: 'iPhone 15 Pro Max',
    description: 'Məhsul təsviri',
    price: 5000.00,
    currency: 'AZN',
    sku: 'IPHONE-15-PRO-MAX-256',
    categoryId: 'category-guid-here',
    brandId: 'brand-guid-here',
    vatRate: 18.00,
    stock: 50
  })
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Məhsul yaradıldı:', data.value);
}
```

---

### 2. Məhsul Yeniləmək

**Endpoint:** `PUT /api/products/{id}`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `id` | Guid | Məhsul ID-si |

**Request Body:**
```json
{
  "name": "iPhone 15 Pro Max (Yenilənmiş)",
  "description": "Yenilənmiş təsvir",
  "price": 4800.00,
  "currency": "AZN",
  "categoryId": "guid",
  "brandId": "guid",
  "vatRate": 18.00,
  "stock": 60
}
```

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": {
    "id": "guid",
    "name": "iPhone 15 Pro Max (Yenilənmiş)",
    "price": 4800.00,
    "currency": "AZN",
    "sku": "IPHONE-15-PRO-MAX-256",
    "categoryName": "Smartfonlar",
    "brandName": "Apple",
    "stock": 60,
    "isActive": true,
    "imageUrl": "/api/images/guid",
    "finalDiscountPercent": 15.5,
    "finalPrice": 4056.00
  }
}
```

---

### 3. Məhsul Silmək

**Endpoint:** `DELETE /api/products/{id}`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `id` | Guid | Məhsul ID-si |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

---

### 4. Məhsul Qiymətini Dəyişmək

**Endpoint:** `PATCH /api/products/{productId}/price`

**Authentication:** Tələb olunur (JWT Token)

**Request Body:**
```json
{
  "newPrice": 4500.00
}
```

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

---

### 5. Məhsul Stokunu Dəyişmək

**Endpoint:** `PATCH /api/products/{productId}/stock`

**Authentication:** Tələb olunur (JWT Token)

**Request Body:**
```json
{
  "quantity": 100,
  "operation": "Increase"
}
```

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

---

### 6. Məhsul Şəkli Yükləmək

**Endpoint:** `POST /api/products/{productId}/image`

**Authentication:** Tələb olunur (JWT Token)

**Content-Type:** `multipart/form-data`

**Form Data:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `file` | File | Yüklənəcək şəkil faylı |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": {
    "id": "guid",
    "imageId": "guid",
    "imageUrl": "/api/images/guid"
  }
}
```

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const productId = 'product-guid-here';
const fileInput = document.querySelector('input[type="file"]');
const file = fileInput.files[0];

const formData = new FormData();
formData.append('file', file);

const response = await fetch(`https://your-api-domain.com/api/products/${productId}/image`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${accessToken}`
  },
  body: formData
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Şəkil yükləndi:', data.value.imageUrl);
}
```

---

## Brend İdarəetməsi (Brand Management)

### 1. Brend Yeniləmək (Promotional Status daxil)

**Endpoint:** `PUT /api/brands/{id}`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `id` | Guid | Brend ID-si |

**Request Body:**
```json
{
  "name": "Sony",
  "isPromotional": true,
  "displayOrder": 1
}
```

**Request Body Parametrləri:**
| Parametr | Tip | Tələb olunur | Təsvir |
|----------|-----|--------------|--------|
| `name` | string | **Bəli** | Brend adı |
| `isPromotional` | bool | Yox | Brendin promotional olub-olmadığı (null ola bilər) |
| `displayOrder` | int? | Yox | Promotional brendlərin sıralaması (0 və ya daha böyük) |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": {
    "id": "guid",
    "name": "Sony",
    "discountPercent": 10.0,
    "isPromotional": true,
    "displayOrder": 1,
    "createdAt": "2025-01-01T00:00:00Z"
  }
}
```

**Qeydlər:**
- `isPromotional` və `displayOrder` optional parametrlərdir
- Yalnız `name` göndərsəniz, promotional status dəyişməz
- `isPromotional: true` göndərdikdə, brend promotional olaraq işarələnir
- `isPromotional: false` göndərdikdə, brend promotional-dan çıxarılır
- `displayOrder` promotional brendlərin sıralamasını təyin edir (kiçik rəqəm = daha yüksək prioritet)

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const brandId = 'brand-guid-here';

// Brendi promotional olaraq işarələ
const response = await fetch(`https://your-api-domain.com/api/brands/${brandId}`, {
  method: 'PUT',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${accessToken}`
  },
  body: JSON.stringify({
    name: 'Sony',
    isPromotional: true,
    displayOrder: 1
  })
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Brend promotional olaraq işarələndi:', data.value);
}
```

**React Nümunəsi:**
```jsx
function BrandPromotionalToggle({ brand }) {
  const [isPromotional, setIsPromotional] = useState(brand.isPromotional);
  const [displayOrder, setDisplayOrder] = useState(brand.displayOrder || 1);
  const [loading, setLoading] = useState(false);
  const accessToken = localStorage.getItem('accessToken');

  const handleUpdate = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `https://your-api-domain.com/api/brands/${brand.id}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
          },
          body: JSON.stringify({
            name: brand.name,
            isPromotional: isPromotional,
            displayOrder: isPromotional ? displayOrder : null
          })
        }
      );

      const data = await response.json();
      if (data.isSuccess) {
        alert('Brend yeniləndi');
      } else {
        alert('Xəta: ' + data.error.message);
      }
    } catch (error) {
      console.error('Xəta:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="brand-promotional-toggle">
      <label>
        <input
          type="checkbox"
          checked={isPromotional}
          onChange={(e) => setIsPromotional(e.target.checked)}
        />
        Promotional Brend
      </label>
      
      {isPromotional && (
        <label>
          Display Order:
          <input
            type="number"
            min="0"
            value={displayOrder}
            onChange={(e) => setDisplayOrder(parseInt(e.target.value))}
          />
        </label>
      )}
      
      <button onClick={handleUpdate} disabled={loading}>
        {loading ? 'Yüklənir...' : 'Yenilə'}
      </button>
    </div>
  );
}
```

---

## Banner və Featured Məhsul İdarəetməsi

### 1. Məhsulu Banner Olaraq Təyin Etmək

**Endpoint:** `POST /api/products/{productId}/banner`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `productId` | Guid | Banner olaraq təyin ediləcək məhsul ID-si |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

**Qeyd:** Əgər başqa bir məhsul artıq banner-dırsa, o avtomatik olaraq banner-dan çıxarılacaq və yeni məhsul banner olaraq təyin ediləcək.

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const productId = 'product-guid-here';

const response = await fetch(`https://your-api-domain.com/api/products/${productId}/banner`, {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Məhsul banner olaraq təyin edildi');
}
```

**React Nümunəsi:**
```jsx
function SetBannerButton({ productId }) {
  const [loading, setLoading] = useState(false);
  const accessToken = localStorage.getItem('accessToken');

  const handleSetBanner = async () => {
    setLoading(true);
    try {
      const response = await fetch(
        `https://your-api-domain.com/api/products/${productId}/banner`,
        {
          method: 'POST',
          headers: {
            'Authorization': `Bearer ${accessToken}`
          }
        }
      );

      const data = await response.json();
      if (data.isSuccess) {
        alert('Məhsul banner olaraq təyin edildi');
      } else {
        alert('Xəta: ' + data.error.message);
      }
    } catch (error) {
      console.error('Xəta:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <button onClick={handleSetBanner} disabled={loading}>
      {loading ? 'Yüklənir...' : 'Banner Olaraq Təyin Et'}
    </button>
  );
}
```

---

### 2. Məhsulu Banner-dan Çıxarmaq

**Endpoint:** `DELETE /api/products/{productId}/banner`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `productId` | Guid | Banner-dan çıxarılacaq məhsul ID-si |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const productId = 'product-guid-here';

const response = await fetch(`https://your-api-domain.com/api/products/${productId}/banner`, {
  method: 'DELETE',
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Məhsul banner-dan çıxarıldı');
}
```

---

### 3. Məhsulu Featured Olaraq Təyin Etmək (Əsas Səhifə üçün)

**Endpoint:** `POST /api/products/{productId}/featured`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `productId` | Guid | Featured olaraq təyin ediləcək məhsul ID-si |

**Request Body:**
```json
{
  "displayOrder": 1
}
```

**Request Body Parametrləri:**
| Parametr | Tip | Tələb olunur | Təsvir |
|----------|-----|--------------|--------|
| `displayOrder` | int | **Bəli** | Display sırası (1-5 arası) |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

**Qeyd:** 
- Display order 1-5 arasında olmalıdır (maksimum 5 featured məhsul ola bilər)
- Əgər seçilmiş display order artıq başqa bir məhsul tərəfindən istifadə olunursa, o məhsul avtomatik olaraq featured-dan çıxarılacaq

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const productId = 'product-guid-here';
const displayOrder = 1; // 1-5 arası

const response = await fetch(`https://your-api-domain.com/api/products/${productId}/featured`, {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${accessToken}`
  },
  body: JSON.stringify({
    displayOrder: displayOrder
  })
});

const data = await response.json();
if (data.isSuccess) {
  console.log(`Məhsul featured olaraq təyin edildi (Display Order: ${displayOrder})`);
}
```

**React Nümunəsi:**
```jsx
function SetFeaturedProduct({ productId }) {
  const [displayOrder, setDisplayOrder] = useState(1);
  const [loading, setLoading] = useState(false);
  const accessToken = localStorage.getItem('accessToken');

  const handleSetFeatured = async () => {
    if (displayOrder < 1 || displayOrder > 5) {
      alert('Display order 1-5 arasında olmalıdır');
      return;
    }

    setLoading(true);
    try {
      const response = await fetch(
        `https://your-api-domain.com/api/products/${productId}/featured`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${accessToken}`
          },
          body: JSON.stringify({ displayOrder })
        }
      );

      const data = await response.json();
      if (data.isSuccess) {
        alert(`Məhsul featured olaraq təyin edildi (Sıra: ${displayOrder})`);
      } else {
        alert('Xəta: ' + data.error.message);
      }
    } catch (error) {
      console.error('Xəta:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <label>
        Display Order (1-5):
        <input
          type="number"
          min="1"
          max="5"
          value={displayOrder}
          onChange={(e) => setDisplayOrder(parseInt(e.target.value))}
        />
      </label>
      <button onClick={handleSetFeatured} disabled={loading}>
        {loading ? 'Yüklənir...' : 'Featured Olaraq Təyin Et'}
      </button>
    </div>
  );
}
```

---

### 4. Məhsulu Featured-dan Çıxarmaq

**Endpoint:** `DELETE /api/products/{productId}/featured`

**Authentication:** Tələb olunur (JWT Token)

**Path Parameters:**
| Parametr | Tip | Təsvir |
|----------|-----|--------|
| `productId` | Guid | Featured-dan çıxarılacaq məhsul ID-si |

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false
}
```

**İstifadə Nümunəsi:**
```javascript
const accessToken = localStorage.getItem('accessToken');
const productId = 'product-guid-here';

const response = await fetch(`https://your-api-domain.com/api/products/${productId}/featured`, {
  method: 'DELETE',
  headers: {
    'Authorization': `Bearer ${accessToken}`
  }
});

const data = await response.json();
if (data.isSuccess) {
  console.log('Məhsul featured-dan çıxarıldı');
}
```

---

## Error Handling

### Ümumi Error Response Formatı

```json
{
  "isSuccess": false,
  "isFailure": true,
  "error": {
    "code": "Error.Code",
    "message": "Xəta mesajı",
    "type": 2
  }
}
```

### Validasiya Xətaları

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
        "message": "Məhsul ID-si boş ola bilməz",
        "property": "ProductId"
      },
      {
        "code": "Validation.DisplayOrder",
        "message": "Display order 1-5 arasında olmalıdır",
        "property": "DisplayOrder"
      }
    ]
  }
}
```

### HTTP Status Kodları

| Status Code | Təsvir |
|-------------|--------|
| 200 | Uğurlu |
| 400 | Bad Request (Validasiya xətası) |
| 401 | Unauthorized (Token yoxdur və ya keçmişdir) |
| 403 | Forbidden (Yetki yoxdur) |
| 404 | Not Found (Məlumat tapılmadı) |
| 409 | Conflict (Məsələn, duplicate məlumat) |
| 500 | Internal Server Error |

### Xüsusi Error Kodları

#### Banner və Featured üçün:

- `Product.NotFound` - Məhsul tapılmadı
- `Product.BannerNotFound` - Banner məhsul tapılmadı
- `Product.InvalidDisplayOrder` - Display order 1-5 arasında olmalıdır
- `Validation.DisplayOrder` - Display order validasiya xətası

---

## Best Practices

### 1. Token Management

```javascript
// Token-ləri localStorage-a saxla
localStorage.setItem('accessToken', token);
localStorage.setItem('refreshToken', refreshToken);

// Request zamanı token əlavə et
const headers = {
  'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
};

// Token expire olduqda refresh et
async function fetchWithAuth(url, options = {}) {
  const accessToken = localStorage.getItem('accessToken');
  
  let response = await fetch(url, {
    ...options,
    headers: {
      ...options.headers,
      'Authorization': `Bearer ${accessToken}`
    }
  });

  // 401 xətası alarsa, token refresh et
  if (response.status === 401) {
    const refreshToken = localStorage.getItem('refreshToken');
    const refreshResponse = await fetch('https://your-api-domain.com/api/auth/refresh-token', {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ refreshToken })
    });

    const refreshData = await refreshResponse.json();
    if (refreshData.isSuccess) {
      localStorage.setItem('accessToken', refreshData.value.accessToken);
      localStorage.setItem('refreshToken', refreshData.value.refreshToken);
      
      // Yenidən cəhd et
      response = await fetch(url, {
        ...options,
        headers: {
          ...options.headers,
          'Authorization': `Bearer ${refreshData.value.accessToken}`
        }
      });
    }
  }

  return response;
}
```

### 2. Error Handling

```javascript
async function handleApiRequest(url, options = {}) {
  try {
    const response = await fetchWithAuth(url, options);
    const data = await response.json();

    if (!data.isSuccess) {
      // Validasiya xətaları
      if (data.error.errors && data.error.errors.length > 0) {
        data.error.errors.forEach(error => {
          console.error(`${error.property}: ${error.message}`);
        });
      } else {
        // Ümumi xəta
        console.error(data.error.message);
      }
      return null;
    }

    return data.value;
  } catch (error) {
    console.error('Network error:', error);
    return null;
  }
}
```

### 3. Banner və Featured İdarəetməsi

```javascript
// Banner məhsul təyin et
async function setBannerProduct(productId) {
  const result = await handleApiRequest(
    `https://your-api-domain.com/api/products/${productId}/banner`,
    { method: 'POST' }
  );
  
  if (result !== null) {
    console.log('Banner məhsul təyin edildi');
    // UI-ni yenilə
  }
}

// Featured məhsul təyin et
async function setFeaturedProduct(productId, displayOrder) {
  if (displayOrder < 1 || displayOrder > 5) {
    alert('Display order 1-5 arasında olmalıdır');
    return;
  }

  const result = await handleApiRequest(
    `https://your-api-domain.com/api/products/${productId}/featured`,
    {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ displayOrder })
    }
  );
  
  if (result !== null) {
    console.log(`Featured məhsul təyin edildi (Display Order: ${displayOrder})`);
    // UI-ni yenilə
  }
}
```

---

## Admin Panel UI Nümunələri

### Banner Məhsul Seçimi

```jsx
function BannerProductSelector({ products }) {
  const [selectedProduct, setSelectedProduct] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleSetBanner = async (productId) => {
    setLoading(true);
    try {
      const result = await setBannerProduct(productId);
      if (result !== null) {
        setSelectedProduct(productId);
        alert('Banner məhsul təyin edildi');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="banner-selector">
      <h3>Banner Məhsul Seçimi</h3>
      <div className="product-list">
        {products.map(product => (
          <div key={product.id} className="product-item">
            <img src={product.imageUrl} alt={product.name} />
            <h4>{product.name}</h4>
            <button
              onClick={() => handleSetBanner(product.id)}
              disabled={loading || selectedProduct === product.id}
              className={selectedProduct === product.id ? 'active' : ''}
            >
              {selectedProduct === product.id ? 'Banner Seçilib' : 'Banner Olaraq Təyin Et'}
            </button>
          </div>
        ))}
      </div>
    </div>
  );
}
```

### Featured Məhsullar İdarəetməsi

```jsx
function FeaturedProductsManager({ products }) {
  const [featuredProducts, setFeaturedProducts] = useState([]);
  const [loading, setLoading] = useState(false);

  const handleSetFeatured = async (productId, displayOrder) => {
    setLoading(true);
    try {
      const result = await setFeaturedProduct(productId, displayOrder);
      if (result !== null) {
        // Featured məhsulları yenilə
        await loadFeaturedProducts();
        alert(`Məhsul featured olaraq təyin edildi (Sıra: ${displayOrder})`);
      }
    } finally {
      setLoading(false);
    }
  };

  const handleRemoveFeatured = async (productId) => {
    setLoading(true);
    try {
      const result = await handleApiRequest(
        `https://your-api-domain.com/api/products/${productId}/featured`,
        { method: 'DELETE' }
      );
      
      if (result !== null) {
        await loadFeaturedProducts();
        alert('Məhsul featured-dan çıxarıldı');
      }
    } finally {
      setLoading(false);
    }
  };

  const loadFeaturedProducts = async () => {
    const result = await handleApiRequest(
      'https://your-api-domain.com/api/products/featured'
    );
    if (result !== null) {
      setFeaturedProducts(result);
    }
  };

  useEffect(() => {
    loadFeaturedProducts();
  }, []);

  return (
    <div className="featured-manager">
      <h3>Featured Məhsullar (Əsas Səhifə - Maksimum 5)</h3>
      
      <div className="current-featured">
        <h4>Hazırkı Featured Məhsullar:</h4>
        {featuredProducts.map(product => (
          <div key={product.id} className="featured-item">
            <span>{product.displayOrder}. {product.name}</span>
            <button onClick={() => handleRemoveFeatured(product.id)}>
              Çıxar
            </button>
          </div>
        ))}
      </div>

      <div className="available-products">
        <h4>Məhsul Seç və Sıra Təyin Et:</h4>
        {products.map(product => {
          const isFeatured = featuredProducts.some(p => p.id === product.id);
          return (
            <div key={product.id} className="product-item">
              <img src={product.imageUrl} alt={product.name} />
              <h4>{product.name}</h4>
              {!isFeatured && (
                <div>
                  <label>
                    Display Order:
                    <select
                      onChange={(e) => {
                        const order = parseInt(e.target.value);
                        handleSetFeatured(product.id, order);
                      }}
                    >
                      <option value="">Seçin</option>
                      {[1, 2, 3, 4, 5].map(order => (
                        <option key={order} value={order}>
                          {order}
                        </option>
                      ))}
                    </select>
                  </label>
                </div>
              )}
              {isFeatured && (
                <span className="featured-badge">
                  Featured (Sıra: {featuredProducts.find(p => p.id === product.id)?.displayOrder})
                </span>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
```

---

## Əlavə Qeydlər

1. **Banner Məhsul:** Yalnız bir məhsul banner ola bilər. Yeni məhsul banner olaraq təyin edildikdə, əvvəlki banner avtomatik olaraq çıxarılır.

2. **Featured Məhsullar:** Maksimum 5 məhsul featured ola bilər. Hər birinin unikal display order-i olmalıdır (1-5).

3. **Display Order:** Əgər seçilmiş display order artıq istifadə olunursa, o məhsul avtomatik olaraq featured-dan çıxarılır.

4. **Validation:** Bütün endpoint-lər validasiya tətbiq edir. Display order 1-5 arasında olmalıdır.

5. **Error Handling:** Bütün xətalar standart formatda qaytarılır. Frontend-də düzgün error handling tətbiq edilməlidir.

---

## Dəstək

Suallarınız və problemlər üçün:
- Email: support@electroshop.com
- Documentation: https://docs.electroshop.com

---

---

## Promotional Brendlər İdarəetməsi

### 1. Brendi Promotional Olaraq İşarələmək

Brendi promotional olaraq işarələmək üçün `UpdateBrand` endpoint-indən istifadə edin. Ətraflı məlumat üçün yuxarıdakı [Brend İdarəetməsi](#brend-idarəetməsi-brand-management) bölməsinə baxın.

### 2. Promotional Brendləri Görüntüləmək

**Endpoint:** `GET /api/brands/promotional`

**Authentication:** Tələb olunmur (`[AllowAnonymous]`)

Bu endpoint user-side üçündür, amma admin panelində də promotional brendlərin siyahısını görmək üçün istifadə edilə bilər.

**Response (200 OK):**
```json
{
  "isSuccess": true,
  "isFailure": false,
  "value": [
    {
      "brand": {
        "id": "brand-1",
        "name": "Sony",
        "discountPercent": 10.0,
        "createdAt": "2024-01-01T00:00:00Z"
      },
      "featuredProduct": {
        "id": "product-1",
        "name": "Playstation 5",
        "price": 499.99,
        "finalPrice": 449.99,
        "finalDiscountPercent": 10.0,
        "currency": "AZN",
        "sku": "PS5-001",
        "categoryName": "Gaming",
        "brandName": "Sony",
        "stock": 50,
        "imageId": "img-123",
        "isActive": true,
        "isFeatured": true,
        "displayOrder": 1
      }
    }
  ]
}
```

**Qeydlər:**
- Maksimum 4 promotional brend qaytarılır
- Hər brend üçün `isFeatured: true` olan məhsul seçilir
- Əgər brend var amma featured product yoxdursa, o brend skip edilir
- Sıralama `displayOrder` və ya `createdAt`-ə görədir

**Admin Panel UI Nümunəsi:**
```jsx
function PromotionalBrandsManager() {
  const [brands, setBrands] = useState([]);
  const [promotionalBrands, setPromotionalBrands] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    loadBrands();
    loadPromotionalBrands();
  }, []);

  const loadBrands = async () => {
    const response = await fetch('https://your-api-domain.com/api/brands?pageSize=100');
    const data = await response.json();
    if (data.isSuccess) {
      setBrands(data.value);
    }
  };

  const loadPromotionalBrands = async () => {
    const response = await fetch('https://your-api-domain.com/api/brands/promotional');
    const data = await response.json();
    if (data.isSuccess) {
      setPromotionalBrands(data.value);
    }
  };

  const handleSetPromotional = async (brandId, isPromotional, displayOrder) => {
    setLoading(true);
    try {
      const brand = brands.find(b => b.id === brandId);
      const result = await handleApiRequest(
        `https://your-api-domain.com/api/brands/${brandId}`,
        {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${localStorage.getItem('accessToken')}`
          },
          body: JSON.stringify({
            name: brand.name,
            isPromotional: isPromotional,
            displayOrder: isPromotional ? displayOrder : null
          })
        }
      );
      
      if (result !== null) {
        await loadBrands();
        await loadPromotionalBrands();
        alert('Brend yeniləndi');
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="promotional-brands-manager">
      <h3>Promotional Brendlər İdarəetməsi</h3>
      
      <div className="current-promotional">
        <h4>Hazırkı Promotional Brendlər (Maksimum 4):</h4>
        {promotionalBrands.map((item, index) => (
          <div key={item.brand.id} className="promotional-item">
            <span>{item.brand.displayOrder || index + 1}. {item.brand.name}</span>
            <span>Featured Product: {item.featuredProduct.name}</span>
            <button onClick={() => handleSetPromotional(item.brand.id, false, null)}>
              Promotional-dan Çıxar
            </button>
          </div>
        ))}
      </div>

      <div className="available-brands">
        <h4>Bütün Brendlər:</h4>
        {brands.map(brand => {
          const isPromotional = promotionalBrands.some(pb => pb.brand.id === brand.id);
          const promotionalItem = promotionalBrands.find(pb => pb.brand.id === brand.id);
          
          return (
            <div key={brand.id} className="brand-item">
              <h5>{brand.name}</h5>
              {!isPromotional && promotionalBrands.length < 4 && (
                <div>
                  <label>
                    Display Order:
                    <select
                      onChange={(e) => {
                        const order = parseInt(e.target.value);
                        if (order) {
                          handleSetPromotional(brand.id, true, order);
                        }
                      }}
                    >
                      <option value="">Seçin</option>
                      {[1, 2, 3, 4].map(order => (
                        <option key={order} value={order}>
                          {order}
                        </option>
                      ))}
                    </select>
                  </label>
                </div>
              )}
              {isPromotional && (
                <div>
                  <span className="promotional-badge">
                    Promotional (Sıra: {promotionalItem?.brand.displayOrder || 'N/A'})
                  </span>
                  {promotionalItem?.featuredProduct ? (
                    <span>Featured: {promotionalItem.featuredProduct.name}</span>
                  ) : (
                    <span className="warning">
                      ⚠️ Bu brend üçün featured məhsul yoxdur!
                    </span>
                  )}
                </div>
              )}
            </div>
          );
        })}
      </div>
    </div>
  );
}
```

**Qeydlər:**
- Promotional brendləri təyin etməzdən əvvəl, həmin brend üçün ən azı bir featured məhsul olmalıdır
- Maksimum 4 brend promotional ola bilər
- Display order 1-4 arasında olmalıdır
- Əgər brend üçün featured product yoxdursa, o brend promotional brands API-də görünməyəcək

---

**Son Yenilənmə:** 2025-11-30
**API Versiya:** v1


