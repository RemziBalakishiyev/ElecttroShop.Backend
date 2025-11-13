using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Primitives;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Contexts;

public class ElectroShopDbContext : DbContext
{
    public ElectroShopDbContext(DbContextOptions<ElectroShopDbContext> options)
        : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Tüm konfiqurasiyaları avtomatik tətbiq et
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ElectroShopDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Audit alanlarını otomatik doldur
        UpdateAuditFields();
        
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        // Audit alanlarını otomatik doldur
        UpdateAuditFields();
        
        return base.SaveChanges();
    }

    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseCommonEntity>();

        foreach (var entry in entries)
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = DateTime.UtcNow;
                // CreatedBy buradan set edilebilir (ICurrentUserService kullanarak)
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = DateTime.UtcNow;
                // UpdatedBy buradan set edilebilir (ICurrentUserService kullanarak)
            }
        }
    }
}
