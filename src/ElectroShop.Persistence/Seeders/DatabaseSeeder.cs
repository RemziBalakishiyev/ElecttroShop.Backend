using ElectroShop.Application.Services;
using ElectroShop.Domain.Entities;
using ElectroShop.Domain.Enums;
using ElectroShop.Domain.ValueObjects;
using ElectroShop.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;

namespace ElectroShop.Persistence.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(ElectroShopDbContext context, IPasswordHasher passwordHasher)
    {
        await SeedUsersAsync(context, passwordHasher);
        await SeedCategoriesAsync(context);
        await SeedBrandsAsync(context);
        await SeedProductsAsync(context);
    }

    private static async Task SeedUsersAsync(ElectroShopDbContext context, IPasswordHasher passwordHasher)
    {
        if (await context.Users.AnyAsync())
            return;

        var adminPasswordHash = passwordHasher.HashPassword("Admin123!");
        var agentPasswordHash = passwordHasher.HashPassword("Agent123!");

        var admin = User.Create("admin@electroshop.az", adminPasswordHash, "Administrator", UserRole.Admin);
        var agent1 = User.Create("agent1@electroshop.az", agentPasswordHash, "Agent 1", UserRole.Agent);
        var agent2 = User.Create("agent2@electroshop.az", agentPasswordHash, "Agent 2", UserRole.Agent);

        context.Users.AddRange(admin, agent1, agent2);
        await context.SaveChangesAsync();
    }

    private static async Task SeedCategoriesAsync(ElectroShopDbContext context)
    {
        if (await context.Categories.AnyAsync())
            return;

        var electronics = Category.Create("Elektronika", null);
        var computers = Category.Create("Kompyuterlər", electronics.Id);
        var smartphones = Category.Create("Smartfonlar", electronics.Id);
        var accessories = Category.Create("Aksessuarlar", electronics.Id);

        var homeAppliances = Category.Create("Məişət Texnikası", null);
        var kitchen = Category.Create("Mətbəx Texnikası", homeAppliances.Id);
        var cleaning = Category.Create("Təmizləmə Texnikası", homeAppliances.Id);

        context.Categories.AddRange(electronics, computers, smartphones, accessories, homeAppliances, kitchen, cleaning);
        await context.SaveChangesAsync();
    }

    private static async Task SeedBrandsAsync(ElectroShopDbContext context)
    {
        if (await context.Brands.AnyAsync())
            return;

        var brands = new[]
        {
            Brand.Create("Apple"),
            Brand.Create("Samsung"),
            Brand.Create("Lenovo"),
            Brand.Create("HP"),
            Brand.Create("Dell"),
            Brand.Create("Sony"),
            Brand.Create("LG"),
            Brand.Create("Bosch"),
            Brand.Create("Philips"),
            Brand.Create("Xiaomi")
        };

        context.Brands.AddRange(brands);
        await context.SaveChangesAsync();
    }

    private static async Task SeedProductsAsync(ElectroShopDbContext context)
    {
        if (await context.Products.AnyAsync())
            return;

        var categories = await context.Categories.ToListAsync();
        var brands = await context.Brands.ToListAsync();

        var electronics = categories.FirstOrDefault(c => c.Name == "Elektronika");
        var computers = categories.FirstOrDefault(c => c.Name == "Kompyuterlər");
        var smartphones = categories.FirstOrDefault(c => c.Name == "Smartfonlar");

        var apple = brands.FirstOrDefault(b => b.Name == "Apple");
        var samsung = brands.FirstOrDefault(b => b.Name == "Samsung");
        var lenovo = brands.FirstOrDefault(b => b.Name == "Lenovo");

        if (electronics == null || computers == null || smartphones == null ||
            apple == null || samsung == null || lenovo == null)
            return;

        var products = new[]
        {
            Product.Create(
                "iPhone 15 Pro Max",
                "IPHONE15PM256",
                smartphones.Id,
                apple.Id,
                2499.99m,
                "AZN",
                0.18m,
                50,
                "Apple iPhone 15 Pro Max 256GB Titanium Blue"),

            Product.Create(
                "Samsung Galaxy S24 Ultra",
                "SGS24U512",
                smartphones.Id,
                samsung.Id,
                2299.99m,
                "AZN",
                0.18m,
                30,
                "Samsung Galaxy S24 Ultra 512GB Phantom Black"),

            Product.Create(
                "Lenovo ThinkPad X1 Carbon",
                "LENOVOX1C",
                computers.Id,
                lenovo.Id,
                2899.99m,
                "AZN",
                0.18m,
                20,
                "Lenovo ThinkPad X1 Carbon Gen 11 Intel Core i7"),

            Product.Create(
                "MacBook Pro 16",
                "MACBOOK16",
                computers.Id,
                apple.Id,
                3499.99m,
                "AZN",
                0.18m,
                15,
                "Apple MacBook Pro 16-inch M3 Pro 512GB"),

            Product.Create(
                "Samsung Galaxy Tab S9",
                "SGTABS9",
                electronics.Id,
                samsung.Id,
                1299.99m,
                "AZN",
                0.18m,
                25,
                "Samsung Galaxy Tab S9 256GB Wi-Fi")
        };

        context.Products.AddRange(products);
        await context.SaveChangesAsync();
    }
}

