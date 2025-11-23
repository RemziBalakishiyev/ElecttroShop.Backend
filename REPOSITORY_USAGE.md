# Repository Pattern - DDD Implementation Guide

## Mimari Yapı

```
┌─────────────────────────────────────────────────┐
│           Application Layer                     │
│  ┌──────────────────────────────────────────┐  │
│  │  IWriteRepository<T>                      │  │
│  │  IQueryRepository<T>                      │  │
│  │  IRepository<T>                           │  │
│  │  IUnitOfWork                              │  │
│  └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│           Persistence Layer                     │
│  ┌──────────────────────────────────────────┐  │
│  │  WriteRepository<T>                       │  │
│  │  QueryRepository<T>                       │  │
│  │  Repository<T>                            │  │
│  │  UnitOfWork                               │  │
│  └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────────────┐
│           Domain Layer                          │
│  ┌──────────────────────────────────────────┐  │
│  │  BaseEntity (with Domain Events)         │  │
│  │  BaseCommonEntity (with Audit fields)    │  │
│  │  AggregateRoot                            │  │
│  │  IDomainEvent (INotification)            │  │
│  └──────────────────────────────────────────┘  │
└─────────────────────────────────────────────────┘
```

## CQRS Separation

### Write Repository (Command)
```csharp
public interface IWriteRepository<TEntity> where TEntity : BaseCommonEntity
{
    Task AddAsync(TEntity entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);
    void Update(TEntity entity);
    void UpdateRange(IEnumerable<TEntity> entities);
    void Delete(TEntity entity);
    void DeleteRange(IEnumerable<TEntity> entities);
    Task DeleteByIdAsync(Guid id, CancellationToken ct = default);
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
```

### Query Repository (Query)
```csharp
public interface IQueryRepository<TEntity> where TEntity : BaseCommonEntity
{
    Task<TEntity?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TEntity> GetByIdOrThrowAsync(Guid id, CancellationToken ct = default);
    Task<List<TEntity>> GetAllAsync(CancellationToken ct = default);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);
    IQueryable<TEntity> AsQueryable();
    IQueryable<TEntity> AsNoTracking();
}
```

## Domain-Driven Design Features

### 1. Domain Events (Otomatik Dispatch)

**SaveChanges Workflow:**
1. Domain Events toplanır (SaveChanges'den ÖNCE)
2. SaveChanges çağrılır
3. Başarılı olursa Domain Events MediatR ile dispatch edilir
4. Events temizlenir

**Örnek Domain Event:**
```csharp
public sealed class ProductPriceChanged : IDomainEvent
{
    public Guid ProductId { get; }
    public decimal OldPrice { get; }
    public decimal NewPrice { get; }
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;

    public ProductPriceChanged(Guid productId, decimal oldPrice, decimal newPrice)
    {
        ProductId = productId;
        OldPrice = oldPrice;
        NewPrice = newPrice;
    }
}
```

**Event Handler:**
```csharp
public class ProductPriceChangedHandler : INotificationHandler<ProductPriceChanged>
{
    public Task Handle(ProductPriceChanged notification, CancellationToken ct)
    {
        // Email gönder
        // Notification oluştur
        // Cache temizle
        // Analytics kaydet
        return Task.CompletedTask;
    }
}
```

**Entity'de Event Fırlatma:**
```csharp
public class Product : BaseCommonEntity
{
    public void ChangePrice(decimal newAmount)
    {
        if (newAmount != Price.Amount)
        {
            var oldPrice = Price.Amount;
            Price = new Money(newAmount, Price.Currency);
            
            // Domain Event ekle
            AddDomainEvent(new ProductPriceChanged(Id, oldPrice, newAmount));
        }
    }
}
```

### 2. Audit Fields (Otomatik Doldurma)

**DbContext Otomatik Handle Eder:**
- `CreatedAtUtc` - Entity eklendiğinde
- `UpdatedAtUtc` - Entity güncellendiğinde
- `CreatedBy` - (ICurrentUserService ile eklenebilir)
- `UpdatedBy` - (ICurrentUserService ile eklenebilir)

```csharp
public abstract class BaseCommonEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; protected set; } = false;

    public void MarkDeleted(string? deletedBy = null)
    {
        IsDeleted = true;
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = deletedBy;
    }
}
```

### 3. Soft Delete (Global Query Filter)

Configuration'da otomatik:
```csharp
builder.HasQueryFilter(e => !e.IsDeleted);
```

Kullanım:
```csharp
// Soft delete
product.MarkDeleted("admin@example.com");
await repository.SaveChangesAsync();

// Deleted kayıtlar otomatik filtrelenir
var products = await repository.GetAllAsync(); // IsDeleted=false olanlar gelir

// Deleted kayıtları da görmek için:
var allProducts = await repository.AsNoTracking()
    .IgnoreQueryFilters()
    .ToListAsync();
```

## Kullanım Örnekleri

### Örnek 1: Command Handler (Write Repository)

```csharp
public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IWriteRepository<Product> _writeRepository;

    public CreateProductCommandHandler(IWriteRepository<Product> writeRepository)
    {
        _writeRepository = writeRepository;
    }

    public async Task<Guid> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var sku = Sku.Create(request.Sku);
        var price = new Money(request.Price, "AZN");

        var product = new Product(
            request.Name,
            sku,
            request.CategoryId,
            request.BrandId,
            price,
            0.18m,
            request.Stock,
            request.Description
        );

        await _writeRepository.AddAsync(product, ct);
        await _writeRepository.SaveChangesAsync(ct);

        return product.Id;
    }
}
```

### Örnek 2: Query Handler (Query Repository)

```csharp
public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IQueryRepository<Product> _queryRepository;

    public GetProductByIdQueryHandler(IQueryRepository<Product> queryRepository)
    {
        _queryRepository = queryRepository;
    }

    public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken ct)
    {
        var product = await _queryRepository
            .AsNoTracking() // Read-only için performans
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct);

        return product == null ? null : MapToDto(product);
    }
}
```

### Örnek 3: Complex Query

```csharp
public class GetActiveProductsQueryHandler : IRequestHandler<GetActiveProductsQuery, List<ProductDto>>
{
    private readonly IQueryRepository<Product> _queryRepository;

    public async Task<List<ProductDto>> Handle(GetActiveProductsQuery request, CancellationToken ct)
    {
        var products = await _queryRepository
            .AsNoTracking()
            .Where(p => p.IsActive && p.Stock > 0)
            .OrderByDescending(p => p.CreatedAtUtc)
            .Take(20)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price.Amount,
                Sku = p.Sku.Value
            })
            .ToListAsync(ct);

        return products;
    }
}
```

### Örnek 4: Unit of Work (Transaction)

```csharp
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWriteRepository<Order> _orderRepo;
    private readonly IQueryRepository<Product> _productQuery;
    private readonly IWriteRepository<Product> _productWrite;

    public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken ct)
    {
        await _unitOfWork.BeginTransactionAsync(ct);
        
        try
        {
            // Ürünleri kontrol et
            var items = new List<OrderItem>();
            foreach (var item in request.Items)
            {
                var product = await _productQuery.GetByIdOrThrowAsync(item.ProductId, ct);
                
                // Stok kontrolü
                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException("Insufficient stock");

                // Stok düş
                product.DecreaseStock(item.Quantity);
                _productWrite.Update(product);

                items.Add(new OrderItem(
                    product.Id,
                    item.Quantity,
                    product.Price,
                    product.VatRate
                ));
            }

            // Sipariş oluştur
            var order = new Order(request.CustomerId, items);
            await _orderRepo.AddAsync(order, ct);

            // Transaction commit
            await _unitOfWork.CommitTransactionAsync(ct);

            return order.Id;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }
}
```

### Örnek 5: Combined Repository (Basit Senaryolar)

```csharp
public class ProductService
{
    private readonly IRepository<Product> _repository;

    public async Task UpdateProductPrice(Guid productId, decimal newPrice, CancellationToken ct)
    {
        // Query ve Write bir arada
        var product = await _repository.GetByIdOrThrowAsync(productId, ct);
        
        product.ChangePrice(newPrice); // Domain Event fırlatır
        
        _repository.Update(product);
        await _repository.SaveChangesAsync(ct); // Event dispatch edilir
    }
}
```

## Domain Events Detaylı Akış

```
1. Entity.ChangePrice(100) 
   → AddDomainEvent(new ProductPriceChanged(...))

2. repository.Update(entity)
   → ChangeTracker'a eklenir

3. repository.SaveChangesAsync()
   ↓
   a. ChangeTracker'dan domain events toplanır
   b. context.SaveChangesAsync() → Database yazılır
   c. MediatR.Publish(domainEvent) → Handler'lar çağrılır
   d. entity.ClearDomainEvents()
```

## Best Practices

1. ✅ **CQRS Kullan**: Write ve Query'leri ayır
2. ✅ **Domain Events**: Business logic'i decouple et
3. ✅ **Unit of Work**: Complex operasyonlar için transaction kullan
4. ✅ **NoTracking**: Read-only query'lerde performans için kullan
5. ✅ **Aggregate Root**: Sadece aggregate root'lar için repository oluştur
6. ✅ **AsNoTracking**: Query'lerde mutlaka kullan
7. ✅ **Soft Delete**: Hard delete yerine soft delete tercih et

## Dependency Injection

```csharp
// Program.cs veya ServiceExtensions.cs
services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
services.AddScoped<IUnitOfWork, UnitOfWork>();

// MediatR
services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
```

## Özet

Bu implementasyon şunları sağlar:
- ✅ Clean Architecture
- ✅ Domain-Driven Design
- ✅ CQRS Pattern
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Domain Events (MediatR)
- ✅ Audit Fields (Otomatik)
- ✅ Soft Delete (Global Filter)
- ✅ Transaction Management
- ✅ Generic & Type-Safe







