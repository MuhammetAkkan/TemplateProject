using App.Application.Interfaces.Data;
using App.Infrastructure.Persistence.Context;
using App.Shared;

namespace App.Infrastructure.Persistence.Repositories;

public class UnitOfWork(AppDbContext context) :IUnitOfWork 
{
    /*
    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
    => await context.SaveChangesAsync(ct);
    */
    public async Task<Result<int>> SaveChangesAsync(CancellationToken ct = default)
    {
        int rowsAffected = await context.SaveChangesAsync(ct);
    
        if (rowsAffected <= 0)
        {
            return Error.Failure("Veritabanına kayıt sırasında hiçbir satır etkilenmedi.");
        }

        // Başarılıysa etkilenen satır sayısını Result içinde dönüyoruz
        return Result<int>.Success(rowsAffected, "İşlem başarıyla kaydedildi.");
    }
}