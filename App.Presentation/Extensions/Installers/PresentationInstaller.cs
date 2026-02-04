using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using App.Infrastructure.Persistence.Errors;
using App.Shared;
using App.Shared.Installers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;

[assembly: ApiConventionType(typeof(DefaultApiConventions))]

namespace App.Presentation.Extensions.Installers;

public class PresentationInstaller : IServiceInstaller
{
    public void Install(WebApplicationBuilder builder)
    {
        // Serilog Yapılandırması (appsettings.json'dan okur)
        builder.Host.UseSerilog((context, configuration) =>
            configuration.ReadFrom.Configuration(context.Configuration));

        // Global Exception Handling
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails();

        // Dapper için Snake_Case eşleşmesi (user_id -> UserId)
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
        
        // Rate Limiting Ayarları
        ConfigureRateLimiting(builder);
        

        // AddControllers içindeki hatalı satırı sildik.
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.WriteIndented = true;
                
            })
            .ConfigureApiBehaviorOptions(options =>
            {
                // 1. Framework'ün kendi kafasına göre 400 dönmesini engelle
                options.SuppressModelStateInvalidFilter = true;

                // 2. Model bağlama (Binding/JSON) hatalarını bizim Result formatına çevir
                options.InvalidModelStateResponseFactory = context =>
                {
                    // Hataları birleştirerek anlamlı bir mesaj oluşturuyoruz
                    var errorMessage = string.Join(" | ", context.ModelState.Values
                        .SelectMany(x => x.Errors)
                        .Select(x => x.ErrorMessage));

                    var error = Error.Validation(errorMessage);
                    var result = Result.Failure(error);

                    return new BadRequestObjectResult(result);
                };
            });
        
        // Diğer ayarlar aynı...
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });

        builder.Services.AddOpenApi("v1", options =>
{
    // 1. ADIM: Şemayı Tanımla (Bu API Bearer Token destekler de)
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Clean Arch API";
        document.Info.Version = "v1";

        // Security Scheme Tanımı
        var securityScheme = new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
        {
            ["Bearer"] = securityScheme
        };

        return Task.CompletedTask;
    });

    // 2. ADIM: Sadece [Authorize] olanlara kilit simgesi koy
    options.AddOperationTransformer((operation, context, cancellationToken) =>
    {
        // Endpoint üzerindeki metadata'ları (Attribute'ları) al
        var metadata = context.Description.ActionDescriptor.EndpointMetadata;

        // [Authorize] var mı?
        var hasAuthorize = metadata.Any(m => m is AuthorizeAttribute);
        // [AllowAnonymous] var mı? (Authorize olsa bile Anonymous varsa kilit koyma)
        var hasAnonymous = metadata.Any(m => m is AllowAnonymousAttribute);

        if (hasAuthorize && !hasAnonymous)
        {
            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer" // Yukarıda tanımladığımız ID ile aynı olmalı
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            };
        }

        return Task.CompletedTask;
    });
});
    }
    
    private static void ConfigureRateLimiting(WebApplicationBuilder builder)
    {
        var rateLimitSettings = builder.Configuration.GetSection("RateLimiting");

        builder.Services.AddRateLimiter(options =>
        {
            // A. Limit aşıldığında dönecek yanıt (429)
            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.HttpContext.Response.ContentType = "application/json";

                // Error.Failure overload metodunu (kod, mesaj) kullandık
                var error = Error.Failure("RateLimit.Exceeded", "Çok fazla istek gönderdiniz. Lütfen biraz bekleyin.");

                var response = new
                {
                    isSuccess = false,
                    error
                };

                await context.HttpContext.Response.WriteAsJsonAsync(response, token);
            };

            // B. Politika Tanımı (Fixed Window)
            options.AddFixedWindowLimiter("fixed", limiterOptions =>
            {
                // JSON'dan oku, bulamazsan varsayılan değerleri (??) kullan (Defensive Coding)
                limiterOptions.PermitLimit = rateLimitSettings.GetValue<int?>("PermitLimit") ?? 5;

                limiterOptions.Window = TimeSpan.FromSeconds(
                    rateLimitSettings.GetValue<int?>("WindowSeconds") ?? 10
                );

                limiterOptions.QueueLimit = rateLimitSettings.GetValue<int?>("QueueLimit") ?? 2;

                limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            });
        });
    }
}

