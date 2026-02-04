using System.Text;
using App.Application.Abstractions.Cache;
using App.Application.Abstractions.Services;
using App.Application.Interfaces.Data;
using App.Domain.Entities;
using App.Infrastructure.Auth;
using App.Infrastructure.Cache;
using App.Infrastructure.Interceptors;
using App.Infrastructure.Persistence.Behaviors;
using App.Infrastructure.Persistence.Context;
using App.Infrastructure.Persistence.Factories;
using App.Infrastructure.Persistence.Repositories;
using App.Infrastructure.Services;
using App.Shared.Installers;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using TS.MediatR;

namespace App.Infrastructure.Installers;

public class InfrastructureInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        // 1. INTERCEPTORS
        builder.Services.AddSingleton<AuditInterceptor>();
        
        // 2. HANGFIRE (Paketi yükleyince burası çalışacak)
        builder.Services.AddHangfire(config => config
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UsePostgreSqlStorage(options =>
                options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("PostgreDefaultConnection")))
        );

        builder.Services.AddHangfireServer();

        // 3. DATABASE
        builder.Services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditInterceptor>();
            options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreDefaultConnection"))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(interceptor);
        });
        
        // 4. REDIS CACHE
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = builder.Configuration.GetConnectionString("Redis"); 
            options.InstanceName = "CleanArch_";
        });
        
        // Redis Cache Service
        builder.Services.AddSingleton<ICacheService, CacheService>();
        
        // hangfire için
        builder.Services.AddScoped<IJobService, JobService>();

        // 5. DAPPER
        builder.Services.AddSingleton<IDbConnectionFactory, PostgreConnectionFactory>();

        // 6. UNIT OF WORK
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // 7. JWT OPTIONS
        // appsettings.json'daki "JwtOptions" ile eşleşmeli
        builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));

        // 8. IDENTITY & AUTHENTICATION (Kritik Bölüm)
        builder.Services.AddIdentity<User,Role>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 6;
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = true;
            options.Lockout.AllowedForNewUsers = false;
        })
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders();

        // ---> EKSİK OLAN JWT BEARER KISMI <---
        // Bu olmazsa token'ı doğrulayamazsın!
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
                ValidAudience = builder.Configuration["JwtOptions:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"]!))
            };
        });

        // 9. MEDIATR PIPELINE
        builder.Services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(App.Application.AssemblyReference).Assembly);
            
            // Sıralama Önemli: Log -> Validation -> Resilience (Polly)
            options.AddOpenBehavior(typeof(LoggingBehavior<,>));
            options.AddOpenBehavior(typeof(ValidationBehavior<,>));
            options.AddOpenBehavior(typeof(ResilienceBehavior<,>));
        });

        // 10. REPOSITORY SCANNING (Otomatik Kayıt)
        builder.Services.Scan(scan => scan
            .FromAssemblies(typeof(InfrastructureInstaller).Assembly)
            .AddClasses(classes => classes.Where(type =>
                (type.Name.EndsWith("Repository") || type.Name.EndsWith("Provider")) &&
                !type.IsAbstract &&
                !type.IsInterface))
            .AsImplementedInterfaces()
            .WithScopedLifetime());
    }
}