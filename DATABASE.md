# Database Sənədi

**DBMS:** PostgreSQL 16  
**ORM:** Entity Framework Core 8.0  
**DbContext:** `ElectroShopDbContext` (`src/ElectroShop.Persistence/Contexts/`)  
**Provider:** Npgsql

---

## Entity iyerarxiyası

```
BaseEntity (Id, DomainEvents)
├── BaseCommonEntity (+ CreatedAtUtc, CreatedBy, UpdatedAtUtc, UpdatedBy, IsDeleted)
│   ├── AggregateRoot (+ RowVersion → xmin)
│   │   ├── Product
│   │   └── Order
│   ├── Category, Brand, Customer, User, Discount
│   ├── ProductVariant, CategoryAttribute
│   ├── RefreshToken
├── OrderItem (audit/soft delete yoxdur)
├── ProductImage (audit/soft delete yoxdur)
└── CategoryAttributeValue (audit/soft delete yoxdur)
```

---

## Cədvəllər

### Products

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | Primary key |
| Name | varchar | Məhsul adı |
| Sku | owned (Value) | SKU kodu |
| Description | text | Təsvir |
| CategoryId | uuid FK | → Categories |
| BrandId | uuid FK | → Brands |
| Price | owned (Money) | Qiymət + valyuta |
| VatRate | decimal | ƏDV faizi (default 18%) |
| Stock | int | Stok miqdarı |
| IsActive | bool | Aktivlik |
| IsBanner | bool | Banner məhsul |
| IsFeatured | bool | Featured məhsul |
| DisplayOrder | int? | Featured sırası (1-5) |
| xmin | xid (row version) | Optimistic concurrency |
| CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy, IsDeleted | audit | Soft delete |

### Categories

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| Name | varchar | Kateqoriya adı |
| Slug | varchar | URL slug (avtomatik generasiya) |
| ParentId | uuid? FK | → Categories (self-reference) |
| audit + IsDeleted | | Soft delete |

### Brands

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| Name | varchar | Brend adı |
| IsPromotional | bool | Promotional brend |
| DisplayOrder | int? | Göstərmə sırası |
| audit + IsDeleted | | Soft delete |

### Customers

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| FullName | varchar | Ad soyad |
| Email | varchar (unique) | E-poçt |
| Phone | varchar (unique, nullable) | Telefon |
| PasswordHash | varchar? | Parol hash (optional) |
| audit + IsDeleted | | Soft delete |

### Users

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| Email | varchar (unique) | Staff e-poçt |
| PasswordHash | varchar | Parol hash |
| FullName | varchar | Ad soyad |
| Role | varchar (enum string) | Admin, Agent |
| IsActive | bool | Aktivlik |
| audit + IsDeleted | | Soft delete |

### Orders

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| CustomerId | uuid FK | → Customers |
| Status | varchar (enum) | Pending, Paid, Processing, Shipped, Delivered, Cancelled, Refunded |
| Subtotal, Vat, Total | owned (Money) | Məbləğlər |
| xmin | xid | Optimistic concurrency |
| audit + IsDeleted | | Soft delete |

### OrderItems

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| OrderId | uuid FK | → Orders (cascade delete) |
| ProductId | uuid FK | → Products |
| Quantity | int | Miqdar |
| UnitPrice | owned (Money) | Vahid qiymət |
| VatRate | decimal | ƏDV faizi |
| LineTotal | owned (Money) | Sətir cəmi |

### Discounts

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| Type | varchar (enum) | Product, Brand, Category |
| ProductId | uuid? FK | → Products |
| BrandId | uuid? FK | → Brands |
| CategoryId | uuid? FK | → Categories |
| Percent | decimal | Endirim faizi (0-100) |
| StartDate | timestamp | Başlanğıc |
| EndDate | timestamp? | Bitmə (null = limitsiz) |
| IsActive | bool | Aktivlik |
| audit + IsDeleted | | Soft delete |

### ProductImages

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| ProductId | uuid FK | → Products (cascade) |
| ImageId | uuid | Şəkil fayl ID-si |
| DisplayOrder | int | Sıra |
| IsPrimary | bool | Əsas şəkil |

### ProductVariants

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| ProductId | uuid FK | → Products (cascade) |
| AttributesJson | text | Variant atributları (JSON) |
| ImageId | uuid? | Variant şəkli |
| IsActive | bool | Aktivlik |
| audit + IsDeleted | | Soft delete |

### CategoryAttributes

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| CategoryId | uuid FK | → Categories (cascade) |
| Name | varchar | Atribut adı |
| DisplayName | varchar | Göstərilən ad |
| AttributeType | varchar | Tip (text, select, color, ...) |
| IsRequired | bool | Məcburi |
| DisplayOrder | int | Sıra |
| audit + IsDeleted | | Soft delete |

### CategoryAttributeValues

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| CategoryAttributeId | uuid FK | → CategoryAttributes (cascade) |
| Value | varchar | Dəyər |
| DisplayValue | varchar? | Göstərilən dəyər |
| DisplayOrder | int | Sıra |
| ColorCode | varchar? | Rəng kodu |

### RefreshTokens

| Sütun | Tip | Təsvir |
|-------|-----|--------|
| Id | uuid PK | |
| UserId | uuid FK | → Users (cascade) |
| Token | varchar | Refresh token |
| ExpiresAt | timestamp | Bitmə tarixi |
| IsRevoked | bool | Ləğv edilib |
| IsUsed | bool | İstifadə edilib |
| RevokedAt | timestamp? | Ləğv tarixi |
| audit + IsDeleted | | Soft delete |

---

## Əlaqələr (Foreign Keys)

```
Categories.ParentId          → Categories.Id        (Restrict)
Products.CategoryId          → Categories.Id        (Restrict)
Products.BrandId             → Brands.Id            (Restrict)
Products → ProductImages      (Cascade)
Products → ProductVariants    (Cascade)
Orders.CustomerId            → Customers.Id           (Restrict)
Orders → OrderItems           (Cascade)
OrderItems.ProductId         → Products.Id            (Restrict)
Discounts.ProductId          → Products.Id            (Restrict)
Discounts.BrandId            → Brands.Id              (Restrict)
Discounts.CategoryId         → Categories.Id          (Restrict)
CategoryAttributes.CategoryId → Categories.Id       (Cascade)
CategoryAttributes → CategoryAttributeValues          (Cascade)
RefreshTokens.UserId         → Users.Id               (Cascade)
```

---

## Audit field-ləri

`BaseCommonEntity` üzərində:

| Field | Təsvir | Avtomatik doldurulma |
|-------|--------|---------------------|
| CreatedAtUtc | Yaradılma tarixi | `SaveChanges` — Added |
| UpdatedAtUtc | Yenilənmə tarixi | `SaveChanges` — Modified |
| CreatedBy | Yaradan istifadəçi | **Doldurulmur** (placeholder) |
| UpdatedBy | Yeniləyən istifadəçi | **Doldurulmur** (placeholder) |
| IsDeleted | Soft delete flag | `MarkDeleted()` metodu |

`ElectroShopDbContext.UpdateAuditFields()` — Added/Modified entity-lərdə avtomatik UTC tarix set edir.

---

## Soft delete məntiqi

1. `BaseCommonEntity.IsDeleted` — default `false`
2. Global query filter: `HasQueryFilter(e => !e.IsDeleted)` — silinmiş qeydlər avtomatik gizlənir
3. Application layer: `entity.MarkDeleted()` — `IsDeleted = true`, `UpdatedAtUtc` set
4. **Qeyd:** `WriteRepository.Delete()` hard delete edir (`Remove`) — soft delete üçün `MarkDeleted()` + `SaveChanges` istifadə olunur

**Soft delete OLMAYAN entity-lər:** OrderItem, ProductImage, CategoryAttributeValue

---

## Optimistic concurrency

- `AggregateRoot.RowVersion` → PostgreSQL `xmin` (xid, IsRowVersion)
- Yalnız `Product` və `Order` cədvəllərində
- Conflict: `ConcurrencyException` → HTTP 409
- Retry: `IUnitOfWork.ReloadAsync()`

---

## Migration tarixçəsi

| Migration | Tarix | Məzmun |
|-----------|-------|--------|
| InitialMigration | 2025-11-13 | Core cədvəllər |
| SomeChanges | 2025-11-21 | ImageId, Phone index |
| AddDiscountTable | 2025-11-23 | Discounts cədvəli |
| AddBannerToProduct | 2025-11-30 | IsBanner, IsFeatured, DisplayOrder |
| AddPromotionalFieldsToBrand | 2025-11-30 | IsPromotional, DisplayOrder |
| AddCategoryAttribute | 2025-11-30 | CategoryAttributes, ProductImages, ProductVariants |
| RemoveSkuPriceStockFromProductVariant | 2025-12-16 | Variant-dan SKU/Price/Stock silindi |
| AddRowVersionToAggregateRoots | 2025-12-20 | RowVersion (bytea) |
| FixRowVersionUsePostgresXmin | 2026-05-23 | xmin istifadəsi |

---

## Enum storage

Bütün enum-lar PostgreSQL-də **string** olaraq saxlanılır:

| Enum | Dəyərlər |
|------|----------|
| UserRole | Admin, Agent |
| OrderStatus | Pending, Paid, Processing, Shipped, Delivered, Cancelled, Refunded |
| DiscountType | Product, Brand, Category |

---

## Value Object mapping

| Entity | Value Object | Owned columns |
|--------|-------------|---------------|
| Product | Sku | Sku_Value |
| Product | Money (Price) | Price_Amount, Price_Currency |
| Order | Money (Subtotal, Vat, Total) | Subtotal_Amount, Subtotal_Currency, ... |
| OrderItem | Money (UnitPrice, LineTotal) | UnitPrice_Amount, ... |

**Dəstəklənən valyutalar:** TRY, USD, EUR, AZN, GBP

---

## Seed məlumatları

`DatabaseSeeder` avtomatik doldurur (yalnız boş cədvəllər):

- 3 User (1 Admin, 2 Agent)
- 7 Category (iyerarxik: Elektronika → Kompyuterlər, Smartfonlar, ...)
- 10 Brand (Apple, Samsung, Lenovo, HP, Dell, Sony, LG, Bosch, Philips, Xiaomi)
- 5 Product (nümunə məhsullar)
