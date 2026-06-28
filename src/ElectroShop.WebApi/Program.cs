using ElectroShop.Application;
using ElectroShop.Persistence;
using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Seeders;
using ElectroShop.WebApi.Extensions;
using ElectroShop.WebApi.Middleware;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Linq;

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

    app.MapGet("/health", () => Results.Ok("OK"));

    var migrateOnStartup = builder.Configuration.GetValue<bool>("MIGRATE_ON_STARTUP");
    if (migrateOnStartup)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ElectroShopDbContext>();
            var passwordHasher = services.GetRequiredService<ElectroShop.Application.Services.IPasswordHasher>();

            var pendingMigrations = context.Database.GetPendingMigrations().ToList();
            if (pendingMigrations.Any())
            {
                Log.Information("Found {Count} pending migration(s): {Migrations}",
                    pendingMigrations.Count, string.Join(", ", pendingMigrations));
                Log.Information("Applying database migrations...");
                await context.Database.MigrateAsync();
                Log.Information("Database migrations applied successfully");
            }
            else
            {
                Log.Information("No pending migrations found");
            }

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
