using System.Reflection;
using FluentValidation;
using Mapster;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
}

