using ElectroShop.Persistence.Contexts;
using ElectroShop.Persistence.Logging;
using ElectroShop.Persistence.Seeders;
using ElectroShop.WebApi.Extensions;
using ElectroShop.WebApi.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.AddWebApiServices();

builder.Host.UseSerilog((context, services, configuration) =>
{
    var logWriter = services.GetRequiredService<IAppLogWriter>();

    configuration
        .ReadFrom.Configuration(context.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("MachineName", Environment.MachineName)
        .WriteTo.Console()
        .WriteTo.Sink(new EfCoreLogSink(logWriter), restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information);
});

try
{
    Log.Information("Starting ElectroShop Web API");

    var app = builder.Build();

    app.ConfigurePipeline();

    app.MapGet("/health", () => Results.Ok("OK"));

    var migrateOnStartup = builder.Configuration.GetValue("MIGRATE_ON_STARTUP", defaultValue: true);
    var seedOnStartup = builder.Environment.IsDevelopment()
        && builder.Configuration.GetValue("SEED_ON_STARTUP", defaultValue: true);

    if (migrateOnStartup || seedOnStartup)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ElectroShopDbContext>();

            if (migrateOnStartup)
            {
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
            }

            if (seedOnStartup)
            {
                var passwordHasher = services.GetRequiredService<ElectroShop.Application.Services.IPasswordHasher>();
                await DatabaseSeeder.SeedAsync(context, passwordHasher);
                Log.Information("Database seeding completed successfully");
            }
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
