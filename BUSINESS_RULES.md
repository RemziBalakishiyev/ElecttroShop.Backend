# Biznes Məntiqi Sənədi

Bu sənəd ElectroShop sistemindəki əsas biznes qaydalarını, condition-ları və user flow-ları izah edir.

---

## 1. Məhsul (Product) modulu

### Yaradılma qaydaları

| Qayda | Mənbə |
|-------|-------|
| Ad 3-200 simvol | `CreateProductCommandValidator` |
| Qiymət 0-1,000,000 | Validator |
| Valyuta: TRY, USD, EUR, AZN, GBP | `Money` value object |
| SKU unikal olmalıdır | Validator + DB |
| SKU format: `^[A-Z0-9\-_]+$`, 3-50 simvol | `Sku` value object |
| Kateqoriya və brend mövcud olmalıdır | Validator |
| ƏDV 0-1 arası (default 18%) | Domain + Validator |
| Stok ≥ 0 | Validator |

### Qiymət və stok

- Qiymət dəyişdikdə `ProductPriceChanged` domain event yaranır
- Stok azaltma: kifayət qədər stok yoxdursa xəta (`DecreaseStock`)
- Default valyuta entity-də AZN, CreateProduct command-də TRY (default)

### Banner məntiqi

- **Yalnız 1 banner məhsul** ola bilər
- Yeni banner təyin edildikdə köhnə banner avtomatik silinir (`SetProductAsBannerCommandHandler`)

### Featured məntiqi

- **Maksimum 5 featured məhsul** (DisplayOrder: 1-5)
- Eyni DisplayOrder slot tutulubsa, köhnə məhsul featured-dan çıxarılır
- Featured olmayan məhsulun DisplayOrder-i yenilənə bilməz

### Variantlar

- Variant atributları JSON formatında (`AttributesJson`)
- Atributlar boş ola bilməz
- Variant silmə = deaktiv etmə (soft delete yox, `IsActive = false`)

### Şəkillər

- Bir məhsulda bir primary şəkil
- Şəkil yükləmə: max 10 MB, `.jpg/.jpeg/.png/.webp/.gif`
- Şəkillər aggregate root vasitəsilə idarə olunur

### Endirim hesablaması

Prioritet: **Məhsul > Brend > Kateqoriya**

```
1. ProductId-yə xüsusi endirim varsa → onu tətbiq et
2. Yoxdursa BrandId endirimi
3. Yoxdursa CategoryId endirimi
4. Heç biri yoxdursa → 0%
```

Endirim aktiv olması: `IsActive = true`, `StartDate <= now`, `EndDate == null || EndDate >= now`

---

## 2. Sifariş (Order) modulu

### Status axını

```
Pending → Paid → Processing → Shipped → Delivered
                ↘ Cancelled
                ↘ Refunded
```

**Hazırda implement edilmiş:** Pending → Paid (`MarkPaid()`)

### Sifariş yaratma

1. Müştəri ID-si tələb olunur
2. Sifariş `Pending` statusu ilə yaradılır
3. Boş sifariş (items = 0) mümkündür

### Element əlavə/silmə

| Qayda | Detay |
|-------|-------|
| Yalnız Pending sifarişlərdə | `Order.AddItem()`, `Order.RemoveItem()` |
| Məhsul aktiv olmalıdır | `AddOrderItemCommandHandler` |
| Stok yoxlanılır | quantity ≤ product.Stock |
| Eyni məhsul varsa | Miqdar artırılır (yeni sətir yox) |
| Məbləğ hesablanması | Subtotal + hər sətir üçün VAT → Total |

### Ödəniş

- `MarkPaid()`: yalnız Pending status, items > 0
- **Payment gateway yoxdur** — manual qeyd

---

## 3. Endirim (Discount) modulu

### Endirim tipləri

| Tip | Tələb olunan FK |
|-----|-----------------|
| Product | ProductId |
| Brand | BrandId |
| Category | CategoryId |

### Validasiya

- Faiz: 0-100
- EndDate > StartDate (EndDate varsa)
- Tipə uyğun ID mütləqdir

### Silmə

- Soft delete / deaktiv etmə (`DeleteDiscountCommandHandler`)

---

## 4. Kateqoriya (Category) modulu

### Slug generasiyası

- Avtomatik: ad əsasında slug yaradılır
- Azərbaycan hərfləri transliterasiya olunur (ə→e, ş→s, ...)

### Atributlar

- Hər kateqoriyaya xüsusi atributlar (rəng, yaddaş, ölçü, ...)
- Atribut tipləri: text, select, color və s.
- Dəyərlər unikal olmalıdır (duplicate rejected)
- Atribut silinməsi cascade ilə dəyərləri də silir

### Lookup cache

- `GET /api/Categories/lookup` — memory cache
- Kateqoriya CRUD zamanı cache invalidation (`LookupCacheInvalidator`)

---

## 5. Brend (Brand) modulu

### Promotional brendlər

- `IsPromotional = true` olan brendlər
- `GET /api/Brands/promotional` — max 4 brend
- Hər brend üçün featured məhsul göstərilir

### Lookup cache

- Kateqoriya ilə eyni cache mexanizmi

---

## 6. Müştəri (Customer) modulu

### Qeydiyyat

- Email unikal olmalıdır
- Ad 2-200 simvol
- **Parol tələb olunmur** — JWT verilmir
- Telefon optional, unique (null filter ilə)

### User flow

```
1. POST /api/Customers/register → Customer ID alınır
2. POST /api/Orders { customerId } → Sifariş yaradılır
3. POST /api/Orders/{id}/items → Məhsullar əlavə edilir
4. PATCH /api/Orders/{id}/mark-paid → Ödəniş qeyd edilir
```

---

## 7. Auth modulu

### Staff login

- Yalnız `User` entity (Admin, Agent, ...)
- Email lowercase normalizasiya
- User: aktiv, silinməmiş olmalıdır
- Yanlış credentials → `Authentication.InvalidCredentials`

### Refresh token

- 64-byte random Base64
- 30 gün etibarlılıq
- Rotation: köhnə token `IsUsed = true`, yeni cüt verilir
- İstifadə edilmiş/ləğv edilmiş/expired token qəbul edilmir

---

## Domain event-lər

### Admin: Məhsul əlavə etmə

```
1. POST /api/Auth/login → Token
2. POST /api/Images/upload (şəkillər) → imageId-lər
3. POST /api/Products → Məhsul + imageIds + variants
4. (Optional) POST /api/Products/{id}/banner
5. (Optional) POST /api/Products/{id}/featured { displayOrder: 1 }
```

### Frontend: Məhsul kataloqu

```
1. GET /api/Products?page=1&categoryId=... → Siyahı (endirimli qiymətlərlə)
2. GET /api/Products/{id} → Detal (şəkillər, variantlar, atributlar)
3. GET /api/Categories/lookup → Filter dropdown
4. GET /api/Brands/lookup → Filter dropdown
```

### Frontend: Ana səhifə

```
1. GET /api/Products/banner → Banner məhsul
2. GET /api/Products/featured → 5 featured məhsul
3. GET /api/Brands/promotional → 4 promotional brend + featured məhsul
```

### Admin: Endirim yaratma

```
1. POST /api/Auth/login → Token
2. POST /api/discounts → Endirim (type + target ID + percent + dates)
3. GET /api/Products → finalDiscountPercent avtomatik hesablanır
```

### Admin: Dashboard

```
1. GET /api/dashboard → Statistika (məhsul sayı, sifariş sayı, gəlir, ...)
2. GET /api/dashboard/chart?period=monthly&periodCount=12 → Qrafik məlumatları
```

---

## Domain event-lər

| Event | Trigger | Handler |
|-------|---------|---------|
| ProductPriceChanged | Product.ChangePrice() / Update() | ProductPriceChangedHandler (log) |

**Gələcək:** Email bildirişi (comment placeholder mövcuddur)

---

## Concurrency qaydaları

- `Product` və `Order` update zamanı `RowVersion` yoxlanılır
- Conflict → HTTP 409, client yenidən oxumalıdır
- `ProductDto.RowVersion` client-ə qaytarılır, update-də göndərilməlidir (dəqiqləşdirilməlidir — handler-də RowVersion istifadəsi yoxlanılmalıdır)
