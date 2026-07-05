using ElectroShop.Application.Abstractions;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Logging;
using ElectroShop.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectroShop.Persistence;

/// <summary>
/// Persistence layer dependency injection
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        var connectionString = PostgreSqlConnectionStringHelper.Normalize(
            configuration.GetConnectionString("DefaultConnection"));

        services.AddDbContext<ElectroShopDbContext>(options =>
            options.UseNpgsql(
                connectionString,
                b => b.MigrationsAssembly(typeof(ElectroShopDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IQueryRepository<>), typeof(QueryRepository<>));
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        
        // Entity-specific repositories
        services.AddScoped<IProductQueryRepository, ProductQueryRepository>();
        services.AddScoped<ICategoryQueryRepository, CategoryQueryRepository>();
        services.AddScoped<IBrandQueryRepository, BrandQueryRepository>();
        services.AddScoped<ICustomerQueryRepository, CustomerQueryRepository>();
        services.AddScoped<IOrderQueryRepository, OrderQueryRepository>();
        services.AddScoped<IDiscountQueryRepository, DiscountQueryRepository>();
        services.AddScoped<ISaleQueryRepository, SaleQueryRepository>();
        services.AddScoped<IProductRatingQueryRepository, ProductRatingQueryRepository>();
        services.AddScoped<IProductImageQueryRepository, ProductImageQueryRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Application logging (database persistence)
        services.AddSingleton<AppLogWriter>();
        services.AddSingleton<IAppLogWriter>(sp => sp.GetRequiredService<AppLogWriter>());
        services.AddHostedService<AppLogPersistenceService>();
        services.AddScoped<IAppLogQueryRepository, AppLogQueryRepository>();

        return services;
    }
}

