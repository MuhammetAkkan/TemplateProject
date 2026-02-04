using App.Domain.Constants;
using App.Domain.Entities;
using App.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace App.Infrastructure.Persistence.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider)
    {
        using var context = new AppDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<AppDbContext>>());

        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        // 1. Veritabanını oluştur (Yoksa)
        await context.Database.EnsureCreatedAsync();

        // 2. --- IDENTITY SEEDING (Guest Kullanıcısı) ---
        User? finalGuestUser = null; 

        // GuestConstant kullanımı:
        var checkGuest = await userManager.FindByEmailAsync(GuestConstant.GuestEmail);

        if (checkGuest == null)
        {
            var newGuest = new User
            {
                UserName = GuestConstant.GuestName,
                Email = GuestConstant.GuestEmail,
                EmailConfirmed = true,
            };

            var result = await userManager.CreateAsync(newGuest, "Guest_123456!");

            if (result.Succeeded)
            {
                Console.WriteLine("👤 Misafir (Guest) kullanıcısı başarıyla oluşturuldu.");
                finalGuestUser = newGuest;
            }
            else
            {
                Console.WriteLine($"Guest oluşturulamadı: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return; // Kritik kullanıcı yoksa devam etme
            }
        }
        else
        {
            Console.WriteLine("ℹ️ Misafir (Guest) kullanıcısı zaten mevcut.");
            finalGuestUser = checkGuest;
        }

        
        // 4. --- STORED PROCEDURES (Her başlangıçta kontrol et/güncelle) ---
        // Bu metod if bloğunun dışına alındı. Böylece veri olsa bile prosedür güncellemesi yapılır.
        await LoadStoredProceduresAsync(context);
    }

    private static async Task LoadStoredProceduresAsync(AppDbContext context)
    {
        // Infrastructure projesi derlendiğinde "Persistence/Procedures" klasörüne kopyalanmalı.
        var baseDir = AppContext.BaseDirectory;
        var proceduresPath = Path.Combine(baseDir, "Persistence", "Procedures");

        if (Directory.Exists(proceduresPath))
        {
            var sqlFiles = Directory.GetFiles(proceduresPath, "*.sql");

            foreach (var filePath in sqlFiles)
            {
                try 
                {
                    string sqlScript = await File.ReadAllTextAsync(filePath);
                    
                    // Dosya içeriği boş değilse çalıştır
                    if (!string.IsNullOrWhiteSpace(sqlScript))
                    {
                        await context.Database.ExecuteSqlRawAsync(sqlScript);
                        Console.WriteLine($"⚙️ Prosedür yüklendi/güncellendi: {Path.GetFileName(filePath)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Hata ({Path.GetFileName(filePath)}): {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine($"⚠️ Uyarı: Prosedür klasörü bulunamadı: {proceduresPath}");
        }
    }
}