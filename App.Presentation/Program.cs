using App.Presentation.Extensions.Installers;
using App.Presentation.Extensions;
using Hangfire;
using Scalar.AspNetCore;
using Serilog;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks; 

var builder = WebApplication.CreateBuilder(args);

// ==================================================================================
// 1. SERVİS KAYITLARI (DEPENDENCY INJECTION)
// ==================================================================================

// Installer Pattern: Katmanlardaki servisleri otomatik yükler
// (JWT, DbContext, Rate Limiting, Serilog, Exception Handling ayarları Installer'larda yapılıyor)
builder.InstallServicesInAssembly(
    typeof(App.Presentation.AssemblyReference).Assembly,
    App.Application.AssemblyReference.Assembly,
    App.Infrastructure.AssemblyReference.Assembly
);

builder.Services.AddHealthChecks()
    // 1. PostgreSQL Kontrolü
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("PostgreDefaultConnection")!,
        name: "PostgreSQL",
        tags: new[] { "db", "sql" })
    // 2. Redis Kontrolü
    .AddRedis(
        redisConnectionString: builder.Configuration.GetConnectionString("Redis")!,
        name: "Redis",
        tags: new[] { "cache", "redis" });


var app = builder.Build();

// ==================================================================================
// 2. HTTP REQUEST PIPELINE (MIDDLEWARE SIRALAMASI)
// ==================================================================================

// 1. Hata Yönetimi (En dışta olmalı ki her şeyi yakalasın)
app.UseExceptionHandler();

// 2. Loglama (İsteklerin loglanması)
app.UseSerilogRequestLogging();

// 3. API Dokümantasyonu (Sadece Geliştirme Ortamı)
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options
            .WithTitle("Clean Arch API")
            .WithTheme(ScalarTheme.BluePlanet)
            .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient);
    });
}

// 4. HTTPS Yönlendirmesi
app.UseHttpsRedirection();

// 5. CORS (KRİTİK: RateLimit ve Auth'dan ÖNCE olmalı)
app.UseCors("AllowAll");

// 6. Rate Limiter (Kötü niyetli istekleri Auth'a girmeden engeller)
app.UseRateLimiter();

// 7. Kimlik Doğrulama (Ben kimim? - Token Kontrolü)
app.UseAuthentication();

// 8. Yetkilendirme (Bunu yapabilir miyim? - Admin/User Kontrolü)
app.UseAuthorization();

// Hangfire Dashboard (Development'ta herkese açık)
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new HangfireAuthorizationFilter() }
});

app.MapHealthChecks("/health", new HealthCheckOptions
{
    // Yanıtı basit "Healthy" yazısı yerine detaylı JSON olarak dönmesi için:
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// 9. Endpoint Yönlendirmesi
app.MapControllers();

app.Run();