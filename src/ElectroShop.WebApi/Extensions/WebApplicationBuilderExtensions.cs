using ElectroShop.Application.Common.Options;
using ElectroShop.Application;
using ElectroShop.Persistence;
using ElectroShop.WebApi.Filters;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

namespace ElectroShop.WebApi.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddWebApiServices(this WebApplicationBuilder builder)
    {
        // Application Layer
        builder.Services.AddApplication();
        builder.Services.AddAuthenticationServices(builder.Configuration);
        builder.Services.AddImageStorage(builder.Configuration);
        builder.Services.AddDiscountServices();

        // Persistence Layer
        builder.Services.AddPersistence(builder.Configuration);

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<Application.Services.ICurrentUserService, Services.CurrentUserService>();

        // Controllers
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

        // CORS — production: FRONTEND_URLS; development fallback: localhost ports
        var allowedOrigins = GetAllowedFrontendOrigins(builder);
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("Frontend", policy =>
            {
                if (allowedOrigins.Length > 0)
                {
                    policy.WithOrigins(allowedOrigins)
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                }
            });
        });

        // JWT Authentication
        var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        if (jwtOptions != null && !string.IsNullOrEmpty(jwtOptions.SigningKey))
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtOptions.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        // Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "ElectroShop API",
                Version = "v1",
                Description = "ElectroShop E-Commerce API"
            });

            // JWT Authentication in Swagger
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });

            // Stream və IFormFile type-larını Swagger schema-dan gizlət
            c.SchemaFilter<SwaggerStreamSchemaFilter>();
            
            // IFormFile parametrlərini düzgün göstərmək üçün
            c.OperationFilter<SwaggerFileOperationFilter>();
            
            // IFormFile və Stream type-larını document-dən tamamilə çıxar
            c.DocumentFilter<SwaggerDocumentFilter>();
        });

        return builder;
    }

    private static string[] GetAllowedFrontendOrigins(WebApplicationBuilder builder)
    {
        var configured = builder.Configuration["FRONTEND_URLS"];
        var origins = string.IsNullOrWhiteSpace(configured)
            ? []
            : configured
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(origin => origin.Trim().TrimEnd('/'))
                .Where(origin => !string.IsNullOrWhiteSpace(origin))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

        if (origins.Length == 0 && builder.Environment.IsDevelopment())
        {
            origins =
            [
                "http://localhost:5173",
                "http://localhost:5174",
                "http://localhost:3000",
                "http://localhost:3001"
            ];
        }

        return origins;
    }
}

