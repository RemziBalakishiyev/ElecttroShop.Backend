using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using ElectroShop.Application.Behaviours;

namespace ElectroShop.Application;

/// <summary>
/// Dependency Injection configuration for Application layer
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Register Memory Cache (for Lookup API-ləri)
        services.AddMemoryCache();

        // Register MediatR
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(assembly);
        });

        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(assembly);

        // Register Mapster
        var mapsterConfig = TypeAdapterConfig.GlobalSettings;
        mapsterConfig.Scan(assembly);
        services.AddSingleton(mapsterConfig);
        services.AddScoped<IMapper, ServiceMapper>();

        // Register Pipeline Behaviours (order matters!)
        // 1. Logging - logs all requests
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));
        
        // 2. Validation - validates requests before processing
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));

        return services;
    }

    public static IServiceCollection AddAuthenticationServices(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        // Register JWT Options
        services.Configure<Common.Options.JwtOptions>(
            configuration.GetSection(Common.Options.JwtOptions.SectionName));

        // Register Services
        services.AddScoped<Services.ITokenService, Services.TokenService>();
        services.AddScoped<Services.IPasswordHasher, Services.PasswordHasher>();

        return services;
    }

    public static IServiceCollection AddImageStorage(
        this IServiceCollection services,
        Microsoft.Extensions.Configuration.IConfiguration configuration)
    {
        // Register Image Storage Service
        services.AddScoped<Services.IImageStorage, Services.LocalImageStorage>();
        
        // Register Image Upload Context (for passing stream to handler)
        services.AddScoped<Services.IImageUploadContext, Services.ImageUploadContext>();

        return services;
    }

    public static IServiceCollection AddDiscountServices(this IServiceCollection services)
    {
        // Register Discount Calculation Service
        services.AddScoped<Services.IDiscountCalculationService, Services.DiscountCalculationService>();

        return services;
    }
}

