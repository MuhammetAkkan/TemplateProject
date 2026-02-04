using App.Shared;

namespace App.Application.Interfaces.Data;

public interface IUnitOfWork
{
    // Repository property'leri YOK! Sadece transaction yönetimi.
    //Task<int> SaveChangesAsync(CancellationToken ct = default);
    
    // Yeni pratik metot
    Task<Result<int>> SaveChangesAsync(CancellationToken ct = default);
}