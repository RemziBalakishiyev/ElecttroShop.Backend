using ElectroShop.Application;
using ElectroShop.Persistence;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Seeders;
using ElectroShop.WebApi.Extensions;
using ElectroShop.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting ElectroShop Web API");

    // Add Web API Services
    builder.AddWebApiServices();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    app.ConfigurePipeline();

    // Database Migration and Seeding
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ElectroShopDbContext>();
            var passwordHasher = services.GetRequiredService<ElectroShop.Application.Services.IPasswordHasher>();

            // Apply migrations (only if pending)
            var runMigrations = builder.Configuration.GetValue<bool>("RunMigrations", false);
            if (runMigrations && context.Database.GetPendingMigrations().Any())
            {
                Log.Information("Applying database migrations...");
                await context.Database.MigrateAsync();
                Log.Information("Database migrations applied successfully");
            }

            // Seed data (always run)
            await DatabaseSeeder.SeedAsync(context, passwordHasher);
            Log.Information("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "An error occurred while migrating or seeding the database");
            throw;
        }
    }

    Log.Information("ElectroShop Web API started successfully");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
