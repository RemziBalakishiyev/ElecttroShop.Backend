    # ElectroShop User Side API Dokumentasiyası

    ## 📋 Mündəricat

    1. [Ümumi Məlumat](#ümumi-məlumat)
    2. [Authentication](#authentication)
    3. [Məhsullar (Products)](#məhsullar-products)
    4. [Kateqoriyalar (Categories)](#kateqoriyalar-categories)
    5. [Brendlər (Brands)](#brendlər-brands)
    6. [Şəkillər (Images)](#şəkillər-images)
    7. [Sifarişlər (Orders)](#sifarişlər-orders)
    8. [Error Handling](#error-handling)

    ---

    ## Ümumi Məlumat

    ### Base URL
    ```
    https://your-api-domain.com/api
    ```

    ### Authentication
    Çoxlu endpoint-lər `[AllowAnonymous]` atributu ilə işarələnib və authentication tələb etmir. Sifariş və müştəri əməliyyatları üçün JWT token tələb olunur.

    ### Response Format
    Bütün API response-ları aşağıdakı formatda qaytarılır:

    **Uğurlu Response:**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": { /* data */ },
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
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

    **Validasiya Xətaları:**
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

    ### Error Types
    - `0` - None
    - `1` - Failure
    - `2` - Validation
    - `3` - NotFound
    - `4` - Conflict
    - `5` - Unauthorized
    - `6` - Forbidden

    ---

    ## Authentication

    ### 1. Login (Giriş)

    **Endpoint:** `POST /api/auth/login`

    **Authentication:** Tələb olunmur

    **Request Body:**
    ```json
    {
    "email": "user@example.com",
    "password": "password123"
    }
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
        "refreshToken": "refresh_token_here",
        "expiresAt": "2025-11-24T12:00:00Z",
        "user": {
        "id": "guid",
        "email": "user@example.com",
        "fullName": "İstifadəçi Adı",
        "role": "Customer",
        "isActive": true,
        "createdAt": "2025-01-01T00:00:00Z"
        }
    }
    }
    ```

    **Xəta Response (400 Bad Request):**
    ```json
    {
    "isSuccess": false,
    "isFailure": true,
    "error": {
        "code": "Auth.InvalidCredentials",
        "message": "Email və ya şifrə yanlışdır",
        "type": 5
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const response = await fetch('https://your-api-domain.com/api/auth/login', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({
        email: 'user@example.com',
        password: 'password123'
    })
    });

    const data = await response.json();
    if (data.isSuccess) {
    // Token-ləri localStorage-a saxla
    localStorage.setItem('accessToken', data.value.accessToken);
    localStorage.setItem('refreshToken', data.value.refreshToken);
    }
    ```

    ---

    ### 2. Refresh Token

    **Endpoint:** `POST /api/auth/refresh-token`

    **Authentication:** Tələb olunmur

    **Request Body:**
    ```json
    {
    "refreshToken": "refresh_token_here"
    }
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "accessToken": "new_access_token",
        "refreshToken": "new_refresh_token",
        "expiresAt": "2025-11-24T12:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const refreshToken = localStorage.getItem('refreshToken');
    const response = await fetch('https://your-api-domain.com/api/auth/refresh-token', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json'
    },
    body: JSON.stringify({ refreshToken })
    });

    const data = await response.json();
    if (data.isSuccess) {
    localStorage.setItem('accessToken', data.value.accessToken);
    localStorage.setItem('refreshToken', data.value.refreshToken);
    }
    ```

    ---

    ## Məhsullar (Products)

    ### 1. Məhsul Siyahısı (Səhifələnmiş)

    **Endpoint:** `GET /api/products`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Query Parameters:**
    | Parametr | Tip | Tələb olunur | Default | Təsvir |
    |----------|-----|--------------|---------|--------|
    | `page` | int | Yox | 1 | Səhifə nömrəsi |
    | `pageSize` | int | Yox | 10 | Hər səhifədə məhsul sayı |
    | `searchTerm` | string | Yox | null | Axtarış termini (ad, SKU, təsvir) |
    | `categoryId` | Guid | Yox | null | Kateqoriya ID-si ilə filtrləmə |
    | `brandId` | Guid | Yox | null | Brend ID-si ilə filtrləmə |
    | `minPrice` | decimal | Yox | null | Minimum qiymət |
    | `maxPrice` | decimal | Yox | null | Maksimum qiymət |
    | `isActive` | bool | Yox | null | Aktiv məhsullar (true/false) |

    **Request Nümunəsi:**
    ```
    GET /api/products?page=1&pageSize=20&categoryId=guid&minPrice=100&maxPrice=1000&isActive=true
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "name": "iPhone 15 Pro Max",
        "price": 5000.00,
        "currency": "AZN",
        "sku": "IPHONE-15-PRO-MAX-256",
        "categoryName": "Smartfonlar",
        "brandName": "Apple",
        "stock": 50,
        "isActive": true,
        "imageUrl": "/api/images/guid",
        "finalDiscountPercent": 15.5,
        "finalPrice": 4225.00
        }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const params = new URLSearchParams({
    page: '1',
    pageSize: '20',
    categoryId: 'category-guid-here',
    minPrice: '100',
    maxPrice: '1000',
    isActive: 'true'
    });

    const response = await fetch(`https://your-api-domain.com/api/products?${params}`);
    const data = await response.json();

    if (data.isSuccess) {
    const products = data.value;
    const totalPages = data.totalPages;
    // Məhsulları göstər
    }
    ```

    ---

    ### 2. Məhsul Detalı

    **Endpoint:** `GET /api/products/{id}`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `id` | Guid | Məhsul ID-si |

    **Request Nümunəsi:**
    ```
    GET /api/products/7711cf76-f50f-490d-85f9-691cbda7457c
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "name": "iPhone 15 Pro Max",
        "description": "Məhsul təsviri",
        "price": 5000.00,
        "currency": "AZN",
        "sku": "IPHONE-15-PRO-MAX-256",
        "categoryId": "guid",
        "categoryName": "Smartfonlar",
        "brandId": "guid",
        "brandName": "Apple",
        "vatRate": 18.00,
        "stock": 50,
        "isActive": true,
        "imageId": "guid",
        "imageUrl": "/api/images/guid",
        "finalDiscountPercent": 15.5,
        "finalPrice": 4225.00,
        "createdAt": "2025-01-01T00:00:00Z",
        "updatedAt": "2025-11-20T00:00:00Z"
    }
    }
    ```

    **Xəta Response (404 Not Found):**
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

    **İstifadə Nümunəsi:**
    ```javascript
    const productId = '7711cf76-f50f-490d-85f9-691cbda7457c';
    const response = await fetch(`https://your-api-domain.com/api/products/${productId}`);
    const data = await response.json();

    if (data.isSuccess) {
    const product = data.value;
    // Məhsul detallarını göstər
    console.log(product.name);
    console.log(product.finalPrice); // Endirimli qiymət
    console.log(product.finalDiscountPercent); // Endirim faizi
    }
    ```

    ---

    ### 3. Məhsul Axtarışı

    **Endpoint:** `GET /api/products/search`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Query Parameters:**
    | Parametr | Tip | Tələb olunur | Default | Təsvir |
    |----------|-----|--------------|---------|--------|
    | `searchTerm` | string | **Bəli** | - | Axtarış termini |
    | `page` | int | Yox | 1 | Səhifə nömrəsi |
    | `pageSize` | int | Yox | 20 | Hər səhifədə məhsul sayı |

    **Request Nümunəsi:**
    ```
    GET /api/products/search?searchTerm=iphone&page=1&pageSize=20
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "name": "iPhone 15 Pro Max",
        "price": 5000.00,
        "currency": "AZN",
        "sku": "IPHONE-15-PRO-MAX-256",
        "categoryName": "Smartfonlar",
        "brandName": "Apple",
        "stock": 50,
        "isActive": true,
        "imageUrl": "/api/images/guid",
        "finalDiscountPercent": 15.5,
        "finalPrice": 4225.00
        }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 25,
    "totalPages": 2
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const searchTerm = 'iphone';
    const params = new URLSearchParams({
    searchTerm: searchTerm,
    page: '1',
    pageSize: '20'
    });

    const response = await fetch(`https://your-api-domain.com/api/products/search?${params}`);
    const data = await response.json();

    if (data.isSuccess) {
    const products = data.value;
    // Axtarış nəticələrini göstər
    }
    ```

    ---

    ### 4. Banner Məhsul

    **Endpoint:** `GET /api/products/banner`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "name": "iPhone 15 Pro Max",
        "description": "Məhsul təsviri",
        "price": 5000.00,
        "currency": "AZN",
        "sku": "IPHONE-15-PRO-MAX-256",
        "categoryId": "guid",
        "categoryName": "Smartfonlar",
        "brandId": "guid",
        "brandName": "Apple",
        "vatRate": 18.00,
        "stock": 50,
        "isActive": true,
        "imageId": "guid",
        "imageUrl": "/api/images/guid",
        "isBanner": true,
        "isFeatured": false,
        "displayOrder": null,
        "finalDiscountPercent": 15.5,
        "finalPrice": 4225.00,
        "createdAt": "2025-01-01T00:00:00Z",
        "updatedAt": "2025-11-20T00:00:00Z"
    }
    }
    ```

    **Xəta Response (404 Not Found):**
    ```json
    {
    "isSuccess": false,
    "isFailure": true,
    "error": {
        "code": "Product.BannerNotFound",
        "message": "Banner məhsul tapılmadı.",
        "type": 3
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const response = await fetch('https://your-api-domain.com/api/products/banner');
    const data = await response.json();

    if (data.isSuccess) {
    const bannerProduct = data.value;
    // Banner məhsulu göstər
    }
    ```

    ---

    ### 5. Featured Məhsullar (Əsas Səhifə üçün)

    **Endpoint:** `GET /api/products/featured`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "name": "iPhone 15 Pro Max",
        "price": 5000.00,
        "currency": "AZN",
        "sku": "IPHONE-15-PRO-MAX-256",
        "categoryName": "Smartfonlar",
        "brandName": "Apple",
        "stock": 50,
        "isActive": true,
        "imageUrl": "/api/images/guid",
        "isBanner": false,
        "isFeatured": true,
        "displayOrder": 1,
        "finalDiscountPercent": 15.5,
        "finalPrice": 4225.00
        },
        {
        "id": "guid",
        "name": "Samsung Galaxy S24 Ultra",
        "price": 4500.00,
        "currency": "AZN",
        "sku": "SAMSUNG-S24-ULTRA",
        "categoryName": "Smartfonlar",
        "brandName": "Samsung",
        "stock": 30,
        "isActive": true,
        "imageUrl": "/api/images/guid",
        "isBanner": false,
        "isFeatured": true,
        "displayOrder": 2,
        "finalDiscountPercent": 10.0,
        "finalPrice": 4050.00
        }
    ]
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const response = await fetch('https://your-api-domain.com/api/products/featured');
    const data = await response.json();

    if (data.isSuccess) {
    const featuredProducts = data.value;
    // Featured məhsulları displayOrder-a görə sıralanmış şəkildə göstər
    featuredProducts.forEach(product => {
        console.log(`${product.displayOrder}. ${product.name}`);
    });
    }
    ```

    **React Nümunəsi:**
    ```jsx
    function FeaturedProducts() {
    const [products, setProducts] = useState([]);

    useEffect(() => {
        fetch('https://your-api-domain.com/api/products/featured')
        .then(res => res.json())
        .then(data => {
            if (data.isSuccess) {
            setProducts(data.value);
            }
        });
    }, []);

    return (
        <div className="featured-products">
        <h2>Əsas Səhifə Məhsulları</h2>
        <div className="products-grid">
            {products.map(product => (
            <ProductCard key={product.id} product={product} />
            ))}
        </div>
        </div>
    );
    }
    ```

    ---

    ## Kateqoriyalar (Categories)

    ### 1. Kateqoriya Siyahısı

    **Endpoint:** `GET /api/categories`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Query Parameters:**
    | Parametr | Tip | Tələb olunur | Default | Təsvir |
    |----------|-----|--------------|---------|--------|
    | `page` | int | Yox | 1 | Səhifə nömrəsi |
    | `pageSize` | int | Yox | 10 | Hər səhifədə kateqoriya sayı |
    | `searchTerm` | string | Yox | null | Axtarış termini |
    | `parentId` | Guid | Yox | null | Valideyn kateqoriya ID-si |
    | `includeChildren` | bool | Yox | false | Alt kateqoriyaları daxil et |

    **Request Nümunəsi:**
    ```
    GET /api/categories?page=1&pageSize=20&includeChildren=true
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "name": "Smartfonlar",
        "slug": "smartfonlar",
        "parentId": null,
        "parentName": null,
        "discountPercent": 20.0,
        "createdAt": "2025-01-01T00:00:00Z"
        }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 50,
    "totalPages": 3
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const params = new URLSearchParams({
    page: '1',
    pageSize: '20',
    includeChildren: 'true'
    });

    const response = await fetch(`https://your-api-domain.com/api/categories?${params}`);
    const data = await response.json();

    if (data.isSuccess) {
    const categories = data.value;
    categories.forEach(category => {
        console.log(category.name);
        console.log(category.discountPercent); // Bu kateqoriyaya tətbiq olunan endirim
    });
    }
    ```

    ---

    ### 2. Kateqoriya Detalı (ID ilə)

    **Endpoint:** `GET /api/categories/{id}`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `id` | Guid | Kateqoriya ID-si |

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "name": "Smartfonlar",
        "slug": "smartfonlar",
        "parentId": null,
        "parentName": null,
        "discountPercent": 20.0,
        "createdAt": "2025-01-01T00:00:00Z"
    }
    }
    ```

    ---

    ### 3. Kateqoriya Detalı (Slug ilə)

    **Endpoint:** `GET /api/categories/slug/{slug}`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `slug` | string | Kateqoriya slug-u |

    **Request Nümunəsi:**
    ```
    GET /api/categories/slug/smartfonlar
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "name": "Smartfonlar",
        "slug": "smartfonlar",
        "parentId": null,
        "parentName": null,
        "discountPercent": 20.0,
        "createdAt": "2025-01-01T00:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const slug = 'smartfonlar';
    const response = await fetch(`https://your-api-domain.com/api/categories/slug/${slug}`);
    const data = await response.json();

    if (data.isSuccess) {
    const category = data.value;
    // Kateqoriya detallarını göstər
    // Sonra bu kateqoriyaya aid məhsulları göstər
    const productsResponse = await fetch(
        `https://your-api-domain.com/api/products?categoryId=${category.id}`
    );
    }
    ```

    ---

    ## Brendlər (Brands)

    ### 1. Brend Siyahısı

    **Endpoint:** `GET /api/brands`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Query Parameters:**
    | Parametr | Tip | Tələb olunur | Default | Təsvir |
    |----------|-----|--------------|---------|--------|
    | `page` | int | Yox | 1 | Səhifə nömrəsi |
    | `pageSize` | int | Yox | 10 | Hər səhifədə brend sayı |
    | `searchTerm` | string | Yox | null | Axtarış termini |

    **Request Nümunəsi:**
    ```
    GET /api/brands?page=1&pageSize=20&searchTerm=apple
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "name": "Apple",
        "discountPercent": 15.0,
        "createdAt": "2025-01-01T00:00:00Z"
        }
    ],
    "page": 1,
    "pageSize": 20,
    "totalCount": 30,
    "totalPages": 2
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const params = new URLSearchParams({
    page: '1',
    pageSize: '20'
    });

    const response = await fetch(`https://your-api-domain.com/api/brands?${params}`);
    const data = await response.json();

    if (data.isSuccess) {
    const brands = data.value;
    brands.forEach(brand => {
        console.log(brand.name);
        console.log(brand.discountPercent); // Bu brendə tətbiq olunan endirim
    });
    }
    ```

    ---

    ### 2. Brend Detalı

    **Endpoint:** `GET /api/brands/{id}`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `id` | Guid | Brend ID-si |

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "name": "Apple",
        "discountPercent": 15.0,
        "isPromotional": true,
        "displayOrder": 1,
        "createdAt": "2025-01-01T00:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const brandId = 'brand-guid-here';
    const response = await fetch(`https://your-api-domain.com/api/brands/${brandId}`);
    const data = await response.json();

    if (data.isSuccess) {
    const brand = data.value;
    console.log(brand.name);
    console.log(brand.discountPercent);
    if (brand.isPromotional) {
        console.log(`Promotional brend (Sıra: ${brand.displayOrder})`);
    }
    }
    ```

    ---

    ### 3. Promotional Brendlər (Əsas Səhifə üçün)

    **Endpoint:** `GET /api/brands/promotional`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

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
            "description": "Incredibly powerful CPUs, GPUs, and an SSD with integrated I/O will redefine your PlayStation experience.",
            "price": 499.99,
            "finalPrice": 449.99,
            "finalDiscountPercent": 10.0,
            "currency": "AZN",
            "sku": "PS5-001",
            "categoryId": "cat-1",
            "categoryName": "Gaming",
            "brandId": "brand-1",
            "brandName": "Sony",
            "stock": 50,
            "imageId": "img-123",
            "imageUrl": "/api/images/img-123",
            "isActive": true,
            "isFeatured": true,
            "displayOrder": 1,
            "vatRate": 18.0,
            "createdAt": "2024-01-01T00:00:00Z",
            "updatedAt": "2024-01-01T00:00:00Z",
            "isBanner": false
        }
        },
        {
        "brand": {
            "id": "brand-2",
            "name": "Apple",
            "discountPercent": 5.0,
            "createdAt": "2024-01-01T00:00:00Z"
        },
        "featuredProduct": {
            "id": "product-2",
            "name": "Macbook Air",
            "description": "The new 15-inch Macbook Air with Liquid Retina display. Supercharged by M2 chip.",
            "price": 1299.99,
            "finalPrice": 1234.99,
            "finalDiscountPercent": 5.0,
            "currency": "AZN",
            "sku": "MBA-001",
            "categoryId": "cat-2",
            "categoryName": "Computers",
            "brandId": "brand-2",
            "brandName": "Apple",
            "stock": 30,
            "imageId": "img-456",
            "imageUrl": "/api/images/img-456",
            "isActive": true,
            "isFeatured": true,
            "displayOrder": 1,
            "vatRate": 18.0,
            "createdAt": "2024-01-01T00:00:00Z",
            "updatedAt": "2024-01-01T00:00:00Z",
            "isBanner": false
        }
        }
    ]
    }
    ```

    **Qeydlər:**
    - Maksimum 4 promotional brend qaytarılır
    - İlk 2 brend böyük banner kimi göstərilir
    - 3-4 brendlər kiçik banner kimi göstərilir
    - Hər brend üçün `isFeatured: true` olan məhsul seçilir
    - Əgər brend var amma featured product yoxdursa, o brend skip edilir
    - Sıralama `displayOrder` və ya `createdAt`-ə görədir
    - Əgər heç bir promotional brand yoxdursa, boş array qaytarılır: `[]`

    **İstifadə Nümunəsi:**
    ```javascript
    const response = await fetch('https://your-api-domain.com/api/brands/promotional');
    const data = await response.json();

    if (data.isSuccess) {
    const promotionalBrands = data.value;
    
    // İlk 2 brend böyük banner kimi
    const largeBanners = promotionalBrands.slice(0, 2);
    
    // Qalan brendlər kiçik banner kimi
    const smallBanners = promotionalBrands.slice(2, 4);
    
    largeBanners.forEach(item => {
        console.log(`Böyük Banner: ${item.brand.name}`);
        console.log(`Featured Product: ${item.featuredProduct.name}`);
        console.log(`Endirim: ${item.featuredProduct.finalDiscountPercent}%`);
        console.log(`Qiymət: ${item.featuredProduct.finalPrice} ${item.featuredProduct.currency}`);
    });
    }
    ```

    **React Nümunəsi:**
    ```jsx
    function PromotionalBrands() {
    const [promotionalBrands, setPromotionalBrands] = useState([]);

    useEffect(() => {
        fetch('https://your-api-domain.com/api/brands/promotional')
        .then(res => res.json())
        .then(data => {
            if (data.isSuccess) {
            setPromotionalBrands(data.value);
            }
        });
    }, []);

    const largeBanners = promotionalBrands.slice(0, 2);
    const smallBanners = promotionalBrands.slice(2, 4);

    return (
        <div className="promotional-brands">
        <h2>Promotional Brendlər</h2>
        
        {/* Böyük Bannerlər */}
        <div className="large-banners">
            {largeBanners.map((item, index) => (
            <div key={item.brand.id} className="large-banner">
                <img 
                src={item.featuredProduct.imageId 
                    ? `https://your-api-domain.com/api/images/${item.featuredProduct.imageId}`
                    : '/placeholder.jpg'} 
                alt={item.featuredProduct.name} 
                />
                <div className="banner-content">
                <h3>{item.brand.name}</h3>
                <h4>{item.featuredProduct.name}</h4>
                <p>{item.featuredProduct.description}</p>
                <div className="price">
                    <span className="original-price">
                    {item.featuredProduct.price} {item.featuredProduct.currency}
                    </span>
                    <span className="discounted-price">
                    {item.featuredProduct.finalPrice} {item.featuredProduct.currency}
                    </span>
                    {item.featuredProduct.finalDiscountPercent > 0 && (
                    <span className="discount-badge">
                        -{item.featuredProduct.finalDiscountPercent}%
                    </span>
                    )}
                </div>
                <button>İndi Al</button>
                </div>
            </div>
            ))}
        </div>

        {/* Kiçik Bannerlər */}
        <div className="small-banners">
            {smallBanners.map((item) => (
            <div key={item.brand.id} className="small-banner">
                <img 
                src={item.featuredProduct.imageId 
                    ? `https://your-api-domain.com/api/images/${item.featuredProduct.imageId}`
                    : '/placeholder.jpg'} 
                alt={item.featuredProduct.name} 
                />
                <div className="banner-content">
                <h4>{item.brand.name}</h4>
                <h5>{item.featuredProduct.name}</h5>
                <div className="price">
                    <span className="discounted-price">
                    {item.featuredProduct.finalPrice} {item.featuredProduct.currency}
                    </span>
                    {item.featuredProduct.finalDiscountPercent > 0 && (
                    <span className="discount-badge">
                        -{item.featuredProduct.finalDiscountPercent}%
                    </span>
                    )}
                </div>
                </div>
            </div>
            ))}
        </div>
        </div>
    );
    }
    ```

    **CSS Nümunəsi:**
    ```css
    .promotional-brands {
    padding: 2rem;
    }

    .large-banners {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 2rem;
    margin-bottom: 2rem;
    }

    .large-banner {
    position: relative;
    border-radius: 12px;
    overflow: hidden;
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    }

    .large-banner img {
    width: 100%;
    height: 400px;
    object-fit: cover;
    }

    .banner-content {
    position: absolute;
    bottom: 0;
    left: 0;
    right: 0;
    background: linear-gradient(to top, rgba(0,0,0,0.8), transparent);
    color: white;
    padding: 2rem;
    }

    .small-banners {
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
    }

    .small-banner {
    border-radius: 8px;
    overflow: hidden;
    box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
    }

    .small-banner img {
    width: 100%;
    height: 200px;
    object-fit: cover;
    }

    .price {
    display: flex;
    align-items: center;
    gap: 1rem;
    margin: 1rem 0;
    }

    .original-price {
    text-decoration: line-through;
    opacity: 0.7;
    }

    .discounted-price {
    font-size: 1.5rem;
    font-weight: bold;
    color: #ff6b6b;
    }

    .discount-badge {
    background: #ff6b6b;
    color: white;
    padding: 0.25rem 0.5rem;
    border-radius: 4px;
    font-size: 0.875rem;
    }
    ```

    ---

    ## Şəkillər (Images)

    ### 1. Şəkil Əldə Etmək

    **Endpoint:** `GET /api/images/{imageId}`

    **Authentication:** Tələb olunmur (`[AllowAnonymous]`)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `imageId` | Guid | Şəkil ID-si |

    **Request Nümunəsi:**
    ```
    GET /api/images/7711cf76-f50f-490d-85f9-691cbda7457c
    ```

    **Response (200 OK):**
    - Content-Type: `image/jpeg`, `image/png`, və s.
    - Response body: Şəkil binary data

    **Xəta Response (404 Not Found):**
    - Şəkil tapılmadıqda 404 qaytarılır

    **İstifadə Nümunəsi:**
    ```html
    <!-- HTML-də -->
    <img src="https://your-api-domain.com/api/images/7711cf76-f50f-490d-85f9-691cbda7457c" alt="Product Image" />

    <!-- JavaScript-də -->
    const imageUrl = `https://your-api-domain.com/api/images/${product.imageId}`;
    const img = document.createElement('img');
    img.src = imageUrl;
    ```

    **React Nümunəsi:**
    ```jsx
    function ProductCard({ product }) {
    const imageUrl = product.imageId 
        ? `https://your-api-domain.com/api/images/${product.imageId}`
        : '/placeholder-image.jpg';

    return (
        <div className="product-card">
        <img src={imageUrl} alt={product.name} />
        <h3>{product.name}</h3>
        <p>{product.finalPrice} {product.currency}</p>
        {product.finalDiscountPercent > 0 && (
            <span className="discount">-{product.finalDiscountPercent}%</span>
        )}
        </div>
    );
    }
    ```

    ---

    ## Sifarişlər (Orders)

    > **Qeyd:** Bütün sifariş endpoint-ləri authentication tələb edir. Request header-da JWT token göndərilməlidir.

    ### 1. Sifariş Detalı

    **Endpoint:** `GET /api/orders/{id}`

    **Authentication:** Tələb olunur (JWT Token)

    **Headers:**
    ```
    Authorization: Bearer {accessToken}
    ```

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `id` | Guid | Sifariş ID-si |

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "customerId": "guid",
        "status": "Pending",
        "subtotal": {
        "amount": 1000.00,
        "currency": "AZN"
        },
        "vat": {
        "amount": 180.00,
        "currency": "AZN"
        },
        "total": {
        "amount": 1180.00,
        "currency": "AZN"
        },
        "items": [
        {
            "productId": "guid",
            "productName": "iPhone 15 Pro Max",
            "quantity": 2,
            "unitPrice": {
            "amount": 500.00,
            "currency": "AZN"
            },
            "lineTotal": {
            "amount": 1000.00,
            "currency": "AZN"
            },
            "vatRate": 18.00
        }
        ],
        "createdAt": "2025-11-24T10:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const orderId = 'order-guid-here';
    const accessToken = localStorage.getItem('accessToken');

    const response = await fetch(`https://your-api-domain.com/api/orders/${orderId}`, {
    headers: {
        'Authorization': `Bearer ${accessToken}`
    }
    });

    const data = await response.json();
    if (data.isSuccess) {
    const order = data.value;
    console.log(`Sifariş Status: ${order.status}`);
    console.log(`Ümumi Məbləğ: ${order.total.amount} ${order.total.currency}`);
    }
    ```

    ---

    ### 2. Müştəri Sifarişləri

    **Endpoint:** `GET /api/orders/customer/{customerId}`

    **Authentication:** Tələb olunur (JWT Token)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `customerId` | Guid | Müştəri ID-si |

    **Query Parameters:**
    | Parametr | Tip | Tələb olunur | Default | Təsvir |
    |----------|-----|--------------|---------|--------|
    | `page` | int | Yox | 1 | Səhifə nömrəsi |
    | `pageSize` | int | Yox | 10 | Hər səhifədə sifariş sayı |

    **Request Nümunəsi:**
    ```
    GET /api/orders/customer/guid?page=1&pageSize=10
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": [
        {
        "id": "guid",
        "customerId": "guid",
        "status": "Pending",
        "total": {
            "amount": 1180.00,
            "currency": "AZN"
        },
        "createdAt": "2025-11-24T10:00:00Z"
        }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3
    }
    ```

    ---

    ### 3. Yeni Sifariş Yaradılması

    **Endpoint:** `POST /api/orders`

    **Authentication:** Tələb olunur (JWT Token)

    **Request Body:**
    ```json
    {
    "customerId": "guid"
    }
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "customerId": "guid",
        "status": "Pending",
        "subtotal": {
        "amount": 0.00,
        "currency": "AZN"
        },
        "vat": {
        "amount": 0.00,
        "currency": "AZN"
        },
        "total": {
        "amount": 0.00,
        "currency": "AZN"
        },
        "items": [],
        "createdAt": "2025-11-24T10:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const accessToken = localStorage.getItem('accessToken');
    const customerId = 'customer-guid-here';

    const response = await fetch('https://your-api-domain.com/api/orders', {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
    },
    body: JSON.stringify({
        customerId: customerId
    })
    });

    const data = await response.json();
    if (data.isSuccess) {
    const order = data.value;
    // Sifariş yaradıldı, indi məhsul əlavə edə bilərsiniz
    }
    ```

    ---

    ### 4. Sifarişə Məhsul Əlavə Etmək

    **Endpoint:** `POST /api/orders/{orderId}/items`

    **Authentication:** Tələb olunur (JWT Token)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `orderId` | Guid | Sifariş ID-si |

    **Request Body:**
    ```json
    {
    "productId": "guid",
    "quantity": 2
    }
    ```

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "customerId": "guid",
        "status": "Pending",
        "subtotal": {
        "amount": 1000.00,
        "currency": "AZN"
        },
        "vat": {
        "amount": 180.00,
        "currency": "AZN"
        },
        "total": {
        "amount": 1180.00,
        "currency": "AZN"
        },
        "items": [
        {
            "productId": "guid",
            "productName": "iPhone 15 Pro Max",
            "quantity": 2,
            "unitPrice": {
            "amount": 500.00,
            "currency": "AZN"
            },
            "lineTotal": {
            "amount": 1000.00,
            "currency": "AZN"
            },
            "vatRate": 18.00
        }
        ],
        "createdAt": "2025-11-24T10:00:00Z"
    }
    }
    ```

    **İstifadə Nümunəsi:**
    ```javascript
    const orderId = 'order-guid-here';
    const accessToken = localStorage.getItem('accessToken');

    const response = await fetch(`https://your-api-domain.com/api/orders/${orderId}/items`, {
    method: 'POST',
    headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accessToken}`
    },
    body: JSON.stringify({
        productId: 'product-guid-here',
        quantity: 2
    })
    });

    const data = await response.json();
    if (data.isSuccess) {
    const order = data.value;
    // Sifariş yeniləndi, yeni məbləğ hesablandı
    console.log(`Yeni Ümumi Məbləğ: ${order.total.amount} ${order.total.currency}`);
    }
    ```

    ---

    ### 5. Sifarişdən Məhsul Silmək

    **Endpoint:** `DELETE /api/orders/{orderId}/items/{productId}`

    **Authentication:** Tələb olunur (JWT Token)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `orderId` | Guid | Sifariş ID-si |
    | `productId` | Guid | Məhsul ID-si |

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "customerId": "guid",
        "status": "Pending",
        "total": {
        "amount": 0.00,
        "currency": "AZN"
        },
        "items": [],
        "createdAt": "2025-11-24T10:00:00Z"
    }
    }
    ```

    ---

    ### 6. Sifarişi Ödənilmiş Olaraq Qeyd Etmək

    **Endpoint:** `PATCH /api/orders/{id}/mark-paid`

    **Authentication:** Tələb olunur (JWT Token)

    **Path Parameters:**
    | Parametr | Tip | Təsvir |
    |----------|-----|--------|
    | `id` | Guid | Sifariş ID-si |

    **Response (200 OK):**
    ```json
    {
    "isSuccess": true,
    "isFailure": false,
    "value": {
        "id": "guid",
        "customerId": "guid",
        "status": "Paid",
        "total": {
        "amount": 1180.00,
        "currency": "AZN"
        },
        "items": [...],
        "createdAt": "2025-11-24T10:00:00Z"
    }
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

    Validasiya xətaları zamanı `errors` array-i də qaytarılır:

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
            "code": "Validation.Quantity",
            "message": "Miqdar 0-dan böyük olmalıdır",
            "property": "Quantity"
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

    ### Error Handling Nümunəsi

    ```javascript
    async function makeApiRequest(url, options = {}) {
    try {
        const response = await fetch(url, options);
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

    // İstifadə
    const product = await makeApiRequest('https://your-api-domain.com/api/products/guid');
    if (product) {
    // Məhsul məlumatlarını göstər
    }
    ```

    ---

    ## Endirim Sistemi

    ### Endirim Prioriteti

    Məhsullar üçün endirim hesablanarkən aşağıdakı prioritet tətbiq olunur:

    1. **Məhsula xüsusi endirim** (ən yüksək prioritet)
    2. **Brend endirimi**
    3. **Kateqoriya endirimi** (ən aşağı prioritet)

    ### Məhsul Response-də Endirim Məlumatları

    Hər məhsul response-unda aşağıdakı endirim məlumatları var:

    ```json
    {
    "id": "guid",
    "name": "iPhone 15 Pro Max",
    "price": 5000.00,              // Orijinal qiymət
    "finalDiscountPercent": 15.5,  // Final endirim faizi
    "finalPrice": 4225.00          // Endirimli qiymət
    }
    ```

    ### Kateqoriya və Brend Response-də Endirim

    Kateqoriya və brend response-larında da endirim məlumatları var:

    ```json
    {
    "id": "guid",
    "name": "Smartfonlar",
    "discountPercent": 20.0  // Bu kateqoriyaya tətbiq olunan endirim
    }
    ```

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

    ### 2. Pagination

    ```javascript
    async function loadProducts(page = 1, pageSize = 20) {
    const params = new URLSearchParams({
        page: page.toString(),
        pageSize: pageSize.toString()
    });

    const response = await fetch(`https://your-api-domain.com/api/products?${params}`);
    const data = await response.json();

    if (data.isSuccess) {
        return {
        products: data.value,
        currentPage: data.page,
        totalPages: data.totalPages,
        totalCount: data.totalCount
        };
    }

    return null;
    }
    ```

    ### 3. Image Loading

    ```javascript
    function getImageUrl(imageId) {
    if (!imageId) {
        return '/placeholder-image.jpg';
    }
    return `https://your-api-domain.com/api/images/${imageId}`;
    }

    // React component-də
    <img 
    src={getImageUrl(product.imageId)} 
    alt={product.name}
    onError={(e) => {
        e.target.src = '/placeholder-image.jpg';
    }}
    />
    ```

    ### 4. Error Handling

    ```javascript
    function handleApiError(error) {
    if (error.errors && error.errors.length > 0) {
        // Validasiya xətaları
        return error.errors.map(e => e.message).join(', ');
    }
    return error.message || 'Xəta baş verdi';
    }
    ```

    ---

    ## Test Məlumatları

    ### Test İstifadəçiləri

    Test məqsədi ilə aşağıdakı istifadəçilərdən istifadə edə bilərsiniz:

    | Email | Password | Role |
    |-------|----------|------|
    | admin@example.com | Admin123! | Admin |
    | customer@example.com | Customer123! | Customer |

    > **Qeyd:** Bu məlumatlar development mühitində test üçündür. Production-da bu məlumatlar silinməlidir.

    ---

    ## Əlavə Qeydlər

    1. **CORS:** API CORS policy ilə konfiqurasiya olunub. Frontend domain-ini `appsettings.json`-da təyin etmək lazımdır.

    2. **Rate Limiting:** Production mühitində rate limiting tətbiq edilməlidir.

    3. **Cache:** Məhsul, kateqoriya və brend siyahıları üçün cache strategiyası tətbiq edilə bilər.

    4. **Image Optimization:** Şəkillər üçün CDN və image optimization tətbiq edilməlidir.

    5. **Monitoring:** Production mühitində logging və monitoring sistemləri tətbiq edilməlidir.

    ---

    ## Dəstək

    Suallarınız və problemlər üçün:
    - Email: support@electroshop.com
    - Documentation: https://docs.electroshop.com

    ---

    **Son Yenilənmə:** 2025-11-30
    **API Versiya:** v1

